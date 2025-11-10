
namespace SmartBoleta.API.Controllers.Request;
public record CrearTenantRequest
(
    string? NombreComercial,
    string? Ruc,
    string? LogoUrl,
    string? ColorPrimario,
    string? FaviconUrl
);
