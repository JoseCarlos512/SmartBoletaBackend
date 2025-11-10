
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartBoleta.Domain.IRepositories;
using SmartBoleta.Infrastructure.Repositories;

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

        services.AddScoped<ITenantRepository, TenantRepository>();

        return services;
    }
}