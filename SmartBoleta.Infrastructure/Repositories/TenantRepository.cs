using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    public Task<Tenant?> ObtenerTenant(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Tenants.AsNoTracking()
                                 .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Tenant>> ObtenerTenants(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tenants.AsNoTracking()
                                       .ToListAsync(cancellationToken);
    }
}