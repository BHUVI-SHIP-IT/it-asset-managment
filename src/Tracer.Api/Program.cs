using FluentValidation;
using Hangfire;
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
            Scheme = "bearer", // must be lower case
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

    // ── Health Checks (Doc 10 §6.2, Doc 11 §6.2) ──
    var healthBuilder = builder.Services.AddHealthChecks();

    if (!string.IsNullOrWhiteSpace(sqlConn))
    {
        healthBuilder.AddSqlServer(sqlConn, name: "sql-server", tags: ["ready"]);
    }

    var redisConn = builder.Configuration.GetConnectionString("Redis");
    if (!string.IsNullOrWhiteSpace(redisConn))
    {
        healthBuilder.AddRedis(redisConn, name: "redis", tags: ["ready"]);
    }

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

    // ════════════════════════════════════════
    var app = builder.Build();
    // ════════════════════════════════════════

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tracer API v1"));
    }

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();
    app.UseCors("AllowAngular");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    
    app.UseHangfireDashboard();
    Tracer.Api.HangfireJobsConfig.ScheduleRecurringJobs();

    // Health check endpoints (Doc 11 §6.2).
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false // Liveness: just checks if the process is running.
    });

    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    Log.Information("Starting Tracer API...");
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
