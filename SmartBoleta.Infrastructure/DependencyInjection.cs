
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartBoleta.Domain.Abstractions.Security;
using SmartBoleta.Domain.IRepositories;
using SmartBoleta.Infrastructure.Repositories;
using SmartBoleta.Infrastructure.Security;

namespace SmartBoleta.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<SmartBoletaDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("SqlServerDatabase");
            options.UseSqlServer(connectionString).UseSnakeCaseNamingConvention();
        });

        // Bind options (desde appsettings.json)
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));


        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();


        return services;
    }
}