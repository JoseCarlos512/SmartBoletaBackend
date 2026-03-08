using Microsoft.EntityFrameworkCore;
using SmartBoleta.Domain;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Infrastructure.Repositories;

internal sealed class CargaMasivaRepository : ICargaMasivaRepository
{
    private readonly SmartBoletaDbContext _dbContext;

    public CargaMasivaRepository(SmartBoletaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(CargaMasiva cargaMasiva, CancellationToken cancellationToken = default)
    {
        await _dbContext.CargaMasivas.AddAsync(cargaMasiva, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<CargaMasiva?> ObtenerPorIdConArchivos(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.CargaMasivas
            .Include(c => c.Archivos)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task UpdateAsync(CargaMasiva cargaMasiva, CancellationToken cancellationToken = default)
    {
        _dbContext.CargaMasivas.Update(cargaMasiva);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
