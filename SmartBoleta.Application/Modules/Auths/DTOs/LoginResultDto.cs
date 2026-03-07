namespace SmartBoleta.Application.Modules.Auths.DTOs;

public record LoginResultDto(
    string Token,
    DateTime ExpiresAt,
    Guid UsuarioId,
    Guid TenantId,
    string Nombre,
    string Correo,
    string Rol
);
