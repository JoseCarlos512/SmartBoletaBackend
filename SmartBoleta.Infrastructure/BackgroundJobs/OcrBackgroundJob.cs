using Microsoft.EntityFrameworkCore;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.Enums;

namespace SmartBoleta.Infrastructure.BackgroundJobs;

public class OcrBackgroundJob
{
    private readonly SmartBoletaDbContext _dbContext;
    private readonly IOcrService _ocrService;
    private readonly INotificationService _notificationService;

    public OcrBackgroundJob(
        SmartBoletaDbContext dbContext,
        IOcrService ocrService,
        INotificationService notificationService
    )
    {
        _dbContext = dbContext;
        _ocrService = ocrService;
        _notificationService = notificationService;
    }

    public async Task ProcesarAsync(Guid boletaId)
    {
        var boleta = await _dbContext.Boletas.FirstOrDefaultAsync(b => b.Id == boletaId);
        if (boleta is null)
            return;

        boleta.Estado = BoletaEstado.ProcesandoOcr;
        await _dbContext.SaveChangesAsync();

        var texto = await _ocrService.ExtraerTextoAsync(boleta.ArchivoUrl);
        boleta.ActualizarOcr(texto);
        await _dbContext.SaveChangesAsync();

        await _notificationService.NotificarTenantAsync(
            boleta.TenantId,
            "boleta_procesada",
            new { boletaId = boleta.Id, estado = boleta.Estado.ToString(), boleta.Periodo }
        );
    }
}
