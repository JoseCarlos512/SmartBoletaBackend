using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartBoleta.Domain.IRepositories
{
    public interface ITenantRepository
    {
        Task AddAsync(Tenant tenant, CancellationToken cancellationToken= default);
        Task<List<Tenant>> ObtenerTenants(CancellationToken cancellationToken = default);
        Task<Tenant?> ObtenerTenant(Guid id, CancellationToken cancellationToken = default);
    }
}