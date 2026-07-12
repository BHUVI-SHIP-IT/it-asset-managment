using System.Security.Claims;
using System.Threading.RateLimiting;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;

using Serilog;
using Serilog.Events;
using Tracer.Api;
using Tracer.Api.Middleware;
using Tracer.Application.Common.Behaviors;
using Tracer.Infrastructure;
using Tracer.Persistence;

// ── Serilog Bootstrap (async, minimal — before host builds) ──
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Async(a => a.Console())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog (full pipeline — driven by appsettings.json) ──
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithProperty("Application", "TracerApi")
        // CorrelationId is pushed by CorrelationIdMiddleware into LogContext
    );

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
    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            opts.JsonSerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter(
                    System.Text.Json.JsonNamingPolicy.CamelCase,
                    allowIntegerValues: true));
        });

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
    var redisConn = builder.Configuration.GetConnectionString("Redis");

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

    // ── Response Compression (Gzip + Brotli) ──
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

    // ── Output Caching ──
    builder.Services.AddOutputCache(options =>
    {
        // Read endpoints vary by Authorization header so each user gets their own cached response.
        options.AddPolicy("UserScoped30s", policy =>
            policy.Expire(TimeSpan.FromSeconds(30))
                  .SetVaryByHeader("Authorization"));

        options.AddPolicy("UserScoped15s", policy =>
            policy.Expire(TimeSpan.FromSeconds(15))
                  .SetVaryByHeader("Authorization"));
    });

    // ── Rate Limiting (Token Bucket, per-user, Redis-backed) ──
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Emit Retry-After so clients know when to retry.
        options.OnRejected = async (ctx, ct) =>
        {
            if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                ctx.HttpContext.Response.Headers.RetryAfter =
                    ((int)retryAfter.TotalSeconds).ToString();

            ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await ctx.HttpContext.Response.WriteAsync(
                "{\"code\":\"RateLimit.Exceeded\",\"description\":\"Too many requests. See Retry-After header.\"}",
                cancellationToken: ct);
        };

        // ── Auth (login / refresh) — brute-force protection ──
        // 20 attempts/min per IP. No queue — fail fast.
        options.AddFixedWindowLimiter(RateLimitPolicies.Auth, o =>
        {
            o.Window = TimeSpan.FromMinutes(1);
            o.PermitLimit = 20;
            o.QueueLimit = 0;
        });

        // ── Read endpoints — high throughput GET ──
        // 1,000 req/min per user (falls back to IP for anon). Queue 100 to absorb bursts.
        options.AddPolicy(RateLimitPolicies.Read, ctx =>
        {
            var partitionKey = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? ctx.Connection.RemoteIpAddress?.ToString()
                ?? "anon";
            
            return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 1000,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 100
            });
        });

        // ── Write endpoints — mutation (POST/PUT/DELETE) ──
        // 200 req/min per user, queue 20.
        options.AddSlidingWindowLimiter(RateLimitPolicies.Write, o =>
        {
            o.Window = TimeSpan.FromMinutes(1);
            o.SegmentsPerWindow = 6;
            o.PermitLimit = 200;
            o.QueueLimit = 20;
            o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });

        // ── Reports — CPU-heavy exports ──
        // 10 req/min per user, queue 5.
        options.AddSlidingWindowLimiter(RateLimitPolicies.Reports, o =>
        {
            o.Window = TimeSpan.FromMinutes(1);
            o.SegmentsPerWindow = 6;
            o.PermitLimit = 10;
            o.QueueLimit = 5;
            o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
    });

    // ── Kestrel tuning for high-concurrency ──
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.Limits.MaxConcurrentConnections = 10_000;
        serverOptions.Limits.MaxConcurrentUpgradedConnections = 1_000;
        serverOptions.Limits.MaxRequestBodySize = 1 * 1024 * 1024; // 1 MB
        serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
    });

    // Raise minimum thread-pool threads to avoid starvation at high concurrency.
    ThreadPool.SetMinThreads(
        workerThreads: Environment.ProcessorCount * 8,
        completionPortThreads: Environment.ProcessorCount * 8);

    // ════════════════════════════════════════
    var app = builder.Build();
    // ════════════════════════════════════════

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tracer API v1"));
    }

    // ── Security Headers ──
    app.UseMiddleware<SecurityHeadersMiddleware>();

    // ── Correlation ID (before request logging so the ID appears in all log entries) ──
    app.UseMiddleware<CorrelationIdMiddleware>();

    // ── HTTPS Redirect ──
    app.UseHttpsRedirection();

    // ── Response Compression ──
    app.UseResponseCompression();

    // ── Output Caching ──
    app.UseOutputCache();

    // ── Rate Limiting ──
    app.UseRateLimiter();

    app.UseExceptionHandler();

    // ── Structured request logging — exclude health checks, swagger, and favicon ──
    app.UseSerilogRequestLogging(opts =>
    {
        opts.GetLevel = (ctx, _, ex) =>
        {
            if (ex != null) return LogEventLevel.Error;
            if (ctx.Response.StatusCode >= 500) return LogEventLevel.Error;
            if (ctx.Response.StatusCode >= 400) return LogEventLevel.Warning;

            var path = ctx.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase))
                return LogEventLevel.Verbose; // Effectively suppressed at Warning min-level.

            return LogEventLevel.Information;
        };

        opts.EnrichDiagnosticContext = (diag, ctx) =>
        {
            diag.Set("RequestHost", ctx.Request.Host.Value ?? "unknown");
            diag.Set("RequestScheme", ctx.Request.Scheme);
            if (ctx.User.Identity?.IsAuthenticated == true)
                diag.Set("UserId", ctx.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown");
        };
    });

    app.UseCors("AllowAngular");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // ── Hangfire Dashboard ──
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter()],
        IsReadOnlyFunc = _ => false
    });

    Tracer.Api.HangfireJobsConfig.ScheduleRecurringJobs();

    // ── Health Check Endpoints ──
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false
    });

    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    Log.Information("Starting Tracer API (M7 Hardened — high-concurrency optimized)...");

    // Dev-only: ensure asset-form lookup dropdowns have sample master data.
    // (EF migrations are not applied on startup per Doc 11.)
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Tracer.Persistence.Contexts.TracerDbContext>();
        await Tracer.Persistence.Seed.MasterDataSeedData.EnsureSeededAsync(db);
        await Tracer.Persistence.Seed.UserSeedData.EnsureSeededAsync(db);
        await Tracer.Persistence.Seed.AssetSeedData.EnsureSeededAsync(db);
        Log.Information("Development master-data, sample users, and demo assets ensured.");
    }

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
