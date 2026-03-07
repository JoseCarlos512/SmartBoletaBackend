using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.Abstractions.Security;
using SmartBoleta.Domain.IRepositories;
using SmartBoleta.Infrastructure.BackgroundJobs;
using SmartBoleta.Infrastructure.Hubs;
using SmartBoleta.Infrastructure.Repositories;
using SmartBoleta.Infrastructure.Security;
using SmartBoleta.Infrastructure.Services;

namespace SmartBoleta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServerDatabase")!;

        services.AddDbContext<SmartBoletaDbContext>(options =>
            options.UseSqlServer(connectionString).UseSnakeCaseNamingConvention()
        );

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        // Repositories (EF Core — writes)
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IBoletaRepository, BoletaRepository>();

        // Security
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

        // Storage & OCR
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<IOcrService, LocalOcrService>();

        // Notifications (SignalR)
        services.AddSignalR();
        services.AddScoped<INotificationService, SignalRNotificationService>();

        // Background jobs (Hangfire)
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            })
        );
        services.AddHangfireServer();
        services.AddScoped<IJobScheduler, HangfireJobScheduler>();
        services.AddScoped<OcrBackgroundJob>();

        // Dapper connection factory
        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        return services;
    }
}
