namespace SmartBoleta.Domain.IRepositories;

public interface ITenantRepository
{
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
