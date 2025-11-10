using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Domain;

public class Tenant : BaseEntity
{

    public string? NombreComercial { get; set; }
    public string? Ruc { get; set; }
    public string? LogoUrl { get; set; }
    public string? ColorPrimario { get; set; }
    public string? FaviconUrl { get; set; }
    public bool Estado { get; set; } = true;
    public string? CreatedPor { get; set; }
    public DateTime ActualizadoPor { get; set; } = DateTime.UtcNow;
}


public static class TenantsErrors
{
    public static Error NotFound = new
    (
        "Tenants.NotFound",
        "No existe un tenant con ese ID"
    );
}