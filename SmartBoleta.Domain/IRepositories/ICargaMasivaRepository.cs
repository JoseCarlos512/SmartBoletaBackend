namespace SmartBoleta.Domain.IRepositories;

public interface ICargaMasivaRepository
{
    Task AddAsync(CargaMasiva cargaMasiva, CancellationToken cancellationToken = default);
    Task<CargaMasiva?> ObtenerPorIdConArchivos(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(CargaMasiva cargaMasiva, CancellationToken cancellationToken = default);
}
