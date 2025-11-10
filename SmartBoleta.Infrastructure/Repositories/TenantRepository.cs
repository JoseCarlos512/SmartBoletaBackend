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

    public async Task<List<Tenant>> ObtenerTenants(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tenants.AsNoTracking()
                                       .ToListAsync(cancellationToken);
    }
}