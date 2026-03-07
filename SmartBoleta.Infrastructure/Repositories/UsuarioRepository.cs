using Microsoft.EntityFrameworkCore;
using SmartBoleta.Domain;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SmartBoletaDbContext _dbContext;

    public UsuarioRepository(SmartBoletaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _dbContext.Usuarios.AddAsync(usuario, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Usuario?> ObtenerUsuarioPorCorreo(string correo, CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Correo == correo, cancellationToken);
    }
}
