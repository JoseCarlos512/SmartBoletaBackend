namespace SmartBoleta.Domain.Abstractions;

public interface INotificationService
{
    Task NotificarTenantAsync(Guid tenantId, string evento, object payload, CancellationToken cancellationToken = default);
    Task NotificarUsuarioAsync(Guid usuarioId, string evento, object payload, CancellationToken cancellationToken = default);
}
