using Microsoft.EntityFrameworkCore;
using SmartBoleta.Domain;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Infrastructure.Repositories;

public class BoletaRepository : IBoletaRepository
{
    private readonly SmartBoletaDbContext _dbContext;

    public BoletaRepository(SmartBoletaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Boleta boleta, CancellationToken cancellationToken = default)
    {
        await _dbContext.Boletas.AddAsync(boleta, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Boleta?> ObtenerPorId(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Boletas.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task UpdateAsync(Boleta boleta, CancellationToken cancellationToken = default)
    {
        _dbContext.Boletas.Update(boleta);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
