using SmartBoleta.Domain;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly SmartBoletaDbContext _dbContext;

    public TenantRepository(SmartBoletaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _dbContext.Tenants.AddAsync(tenant, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
