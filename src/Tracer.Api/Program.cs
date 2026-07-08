using System.Threading.RateLimiting;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Tracer.Application.Common.Behaviors;
using Tracer.Infrastructure;
using Tracer.Persistence;

// ── Serilog Bootstrap ──
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ──
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // ── Layer DI Registrations ──
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── MediatR + FluentValidation ──
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(ValidationBehavior<,>).Assembly);
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });

    builder.Services.AddValidatorsFromAssembly(typeof(ValidationBehavior<,>).Assembly);

    // ── Controllers ──
    builder.Services.AddControllers();

    // ── Swagger/OpenAPI (Dev only) ──
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Tracer API",
            Version = "v1",
            Description = "Enterprise IT Asset Management System — Project Tracer"
        });

        var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "JWT Authentication",
            Description = "Enter JWT Bearer token **_only_**",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Id = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
            }
        };
        options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {securityScheme, Array.Empty<string>()}
        });
    });

    var sqlConn = builder.Configuration.GetConnectionString("DefaultConnection");

    // ── Hangfire ──
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(Hangfire.CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(sqlConn));
    builder.Services.AddHangfireServer();
    builder.Services.AddScoped<Tracer.Api.BackgroundServices.ProcessOutboxMessagesJob>();

    // ── Health Checks ──
    var healthBuilder = builder.Services.AddHealthChecks();

    if (!string.IsNullOrWhiteSpace(sqlConn))
        healthBuilder.AddSqlServer(sqlConn, name: "sql-server", tags: ["ready"]);

    var redisConn = builder.Configuration.GetConnectionString("Redis");
    if (!string.IsNullOrWhiteSpace(redisConn))
        healthBuilder.AddRedis(redisConn, name: "redis", tags: ["ready"]);

    // ── CORS ──
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
        {
            var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:4200"];
            policy.WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // ── Exception Handler ──
    builder.Services.AddExceptionHandler<Tracer.Api.Middleware.GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // ── M7: Response Compression (Gzip + Brotli for JSON API responses) ──
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            ["application/json", "application/problem+json"]);
    });
    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        options.Level = System.IO.Compression.CompressionLevel.Fastest);
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        options.Level = System.IO.Compression.CompressionLevel.SmallestSize);

    // ── M7: Output Caching ──
    builder.Services.AddOutputCache(options =>
    {
        // Read endpoints are cached per-user (vary by Authorization header).
        options.AddPolicy("UserScoped30s", policy =>
            policy.Expire(TimeSpan.FromSeconds(30))
                  .VaryByHeader("Authorization"));

        options.AddPolicy("UserScoped15s", policy =>
            policy.Expire(TimeSpan.FromSeconds(15))
                  .VaryByHeader("Authorization"));
    });

    // ── M7: Rate Limiting (ASP.NET Core 7+ built-in) ──
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Read endpoints: 300 requests per minute per IP.
        options.AddFixedWindowLimiter("ReadPolicy", limiterOptions =>
        {
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.PermitLimit = 300;
            limiterOptions.QueueLimit = 0;
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });

        // Write/mutation endpoints: 100 requests per minute per IP.
        options.AddFixedWindowLimiter("WritePolicy", limiterOptions =>
        {
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.PermitLimit = 100;
            limiterOptions.QueueLimit = 0;
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });

        // Auth endpoint: 20 login attempts per minute per IP (brute-force protection).
        options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
        {
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.PermitLimit = 20;
            limiterOptions.QueueLimit = 0;
            limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
    });

    // ── M7: Maximum Request Body Size (1MB) ──
    builder.WebHost.ConfigureKestrel(serverOptions =>
        serverOptions.Limits.MaxRequestBodySize = 1 * 1024 * 1024); // 1 MB

    // ════════════════════════════════════════
    var app = builder.Build();
    // ════════════════════════════════════════

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tracer API v1"));
    }

    // ── M7: Security Headers (earliest possible — applies to all responses) ──
    app.UseMiddleware<Tracer.Api.Middleware.SecurityHeadersMiddleware>();

    // ── M7: HTTPS Redirect ──
    app.UseHttpsRedirection();

    // ── M7: Response Compression ──
    app.UseResponseCompression();

    // ── M7: Output Caching ──
    app.UseOutputCache();

    // ── M7: Rate Limiting ──
    app.UseRateLimiter();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();
    app.UseCors("AllowAngular");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // ── M7: Hangfire Dashboard — require authentication ──
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter()],
        IsReadOnlyFunc = _ => false
    });

    Tracer.Api.HangfireJobsConfig.ScheduleRecurringJobs();

    // Health check endpoints (Doc 11 §6.2).
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false
    });

    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    Log.Information("Starting Tracer API (M7 Hardened)...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Tracer API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
