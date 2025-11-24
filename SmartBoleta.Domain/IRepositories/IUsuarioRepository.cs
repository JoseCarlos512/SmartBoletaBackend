using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartBoleta.Domain.IRepositories
{
    public interface IUsuarioRepository
    {
        Task AddAsync(Usuario Usuario, CancellationToken cancellationToken= default);
        Task<List<Usuario>> ObtenerUsuarios(CancellationToken cancellationToken = default);
        Task<Usuario?> ObtenerUsuario(Guid id, CancellationToken cancellationToken = default);
        Task<Usuario?> ObtenerUsuarioPorCorreo(string correo, CancellationToken cancellationToken = default);
    }
}