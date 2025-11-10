using System;

namespace SmartBoleta.Application.Modules.Tenants.DTOs;
public class TenantDto
{
    public Guid Id { get; set; } = default;
    public string NombreComercial { get; set; } = default!;
    public string Ruc { get; set; } = default!;
    public string LogoUrl { get; set; } = default!;
    public string ColorPrimario { get; set; } = default!;
    public string FaviconUrl { get; set; } = default!;
}
