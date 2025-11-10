using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Domain;

public class Tenant : BaseEntity
{

    public Tenant() {}

    public Tenant(
        Guid id,
        string nombreComercial,
        string ruc,
        string logoUrl,
        string colorPrimario,
        string faviconUrl
    ) : base(id)
    {
        NombreComercial = nombreComercial;
        Ruc = ruc;
        LogoUrl = logoUrl;
        ColorPrimario = colorPrimario;
        FaviconUrl = faviconUrl;
    }

    public string? NombreComercial { get; set; }
    public string? Ruc { get; set; }
    public string? LogoUrl { get; set; }
    public string? ColorPrimario { get; set; }
    public string? FaviconUrl { get; set; }
    public bool Estado { get; set; } = true;
    public string? CreatedPor { get; set; }
    public DateTime ActualizadoPor { get; set; } = DateTime.UtcNow;

    public static Tenant Create(
        string nombreComercial,
        string ruc,
        string logoUrl,
        string colorPrimario,
        string faviconUrl
    )
    {
        return new Tenant(
            Guid.NewGuid(),
            nombreComercial,
            ruc,
            logoUrl,
            colorPrimario,
            faviconUrl
        );
    }
}


public static class TenantsErrors
{
    public static Error NotFound = new
    (
        "Tenants.NotFound",
        "No existe un tenant con ese ID"
    );
}