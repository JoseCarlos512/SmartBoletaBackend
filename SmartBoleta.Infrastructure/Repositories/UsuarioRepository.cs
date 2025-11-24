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

    public async Task AddAsync(Usuario Usuario, CancellationToken cancellationToken = default)
    {
        await _dbContext.Usuarios.AddAsync(Usuario, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Usuario?> ObtenerUsuario(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios.AsNoTracking()
                                 .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<Usuario?> ObtenerUsuarioPorCorreo(string correo, CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios.AsNoTracking()
                                 .FirstOrDefaultAsync(t => t.Correo == correo, cancellationToken);
    }

    public async Task<List<Usuario>> ObtenerUsuarios(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Usuarios.AsNoTracking()
                                       .ToListAsync(cancellationToken);
    }
}