namespace SmartBoleta.Domain.IRepositories;

public interface IBoletaRepository
{
    Task AddAsync(Boleta boleta, CancellationToken cancellationToken = default);
    Task<Boleta?> ObtenerPorId(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Boleta boleta, CancellationToken cancellationToken = default);
}
