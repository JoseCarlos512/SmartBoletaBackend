using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartBoleta.Domain.IRepositories
{
    public interface ITenantRepository
    {
        Task<List<Tenant>> ObtenerTenants(CancellationToken cancellationToken = default);
    }
}