namespace SmartBoleta.Domain.IRepositories;

public interface IUsuarioRepository
{
    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task<Usuario?> ObtenerUsuarioPorCorreo(string correo, CancellationToken cancellationToken = default);
}
