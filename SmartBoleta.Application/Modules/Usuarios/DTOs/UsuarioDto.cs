using System;

namespace SmartBoleta.Application.Modules.Usuarios.DTOs;
public class UsuarioDto
{
    public Guid Id { get; set; } = default;
    public Guid TenantId { get; set; } = default;
    public string Nombre { get; set; } = default!;
    public string? Correo { get; set; } = default!;
    public string DNI { get; set; } = default!;
    public bool Estado { get; set; } = default!;
}
