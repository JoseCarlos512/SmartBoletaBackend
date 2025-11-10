using SmartBoleta.Application.Abstractions.Messaging;

namespace SmartBoleta.Application.Modules.Tenants.Command;

public record CrearTenantsCommand
(
    string NombreComercial,
    string Ruc,
    string LogoUrl,
    string ColorPrimario,
    string FaviconUrl
) : ICommand<Guid>;


