using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SmartBoleta.Infrastructure.Hubs;

[Authorize]
public class NotificacionHub : Hub
{
    public async Task UnirseATenant(string tenantId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

    public async Task SalirDeTenant(string tenantId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");
}
