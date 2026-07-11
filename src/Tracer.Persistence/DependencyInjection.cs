using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tracer.Application.Common.Interfaces;
using Tracer.Persistence.Contexts;
using Tracer.Persistence.Interceptors;
using Tracer.Persistence.Repositories;

namespace Tracer.Persistence;

/// <summary>
/// Registers all Persistence services: DbContext, interceptors, repositories, UoW.
/// Called from <c>Program.cs</c> via <c>builder.Services.AddPersistence(configuration)</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Interceptors must be singleton — AddDbContextPool resolves from the root provider.
        services.AddSingleton<AuditableEntityInterceptor>();
        services.AddSingleton<OutboxInterceptor>();

        services.AddDbContextPool<TracerDbContext>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            var outboxInterceptor = sp.GetRequiredService<OutboxInterceptor>();

            options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(TracerDbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                        sqlOptions.MaxBatchSize(100);
                    })
                .AddInterceptors(auditInterceptor, outboxInterceptor);
        }, poolSize: 128);


        // Repositories & UoW.
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<TracerDbContext>());

        return services;
    }
}
