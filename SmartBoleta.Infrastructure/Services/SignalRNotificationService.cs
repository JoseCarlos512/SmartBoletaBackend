using Microsoft.AspNetCore.SignalR;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Infrastructure.Hubs;

namespace SmartBoleta.Infrastructure.Services;

internal sealed class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificacionHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificacionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotificarTenantAsync(Guid tenantId, string evento, object payload, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group($"tenant_{tenantId}").SendAsync(evento, payload, cancellationToken);

    public Task NotificarUsuarioAsync(Guid usuarioId, string evento, object payload, CancellationToken cancellationToken = default)
        => _hubContext.Clients.User(usuarioId.ToString()).SendAsync(evento, payload, cancellationToken);
}
