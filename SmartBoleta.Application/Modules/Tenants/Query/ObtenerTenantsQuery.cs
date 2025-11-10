
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Tenants.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Tenants.Query;

public sealed record ObtenerTenantsQuery() : IQuery<List<TenantDto>>;

public class ObtenerTenantsQueryHandler : IQueryHandler<ObtenerTenantsQuery, List<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;
    public ObtenerTenantsQueryHandler(ITenantRepository TenantRepository)
    {
        _tenantRepository = TenantRepository;
    }
    public async Task<Result<List<TenantDto>>> Handle(ObtenerTenantsQuery request, CancellationToken cancellationToken)
    {

        var tenants = await _tenantRepository.ObtenerTenants(cancellationToken);

        if (tenants is null || !tenants.Any())
        {
            return Result.Failure<List<TenantDto>>(TenantsErrors.NotFound);
        }

        var tenantDtos = tenants.Select(t => new TenantDto
        {
            Id = t.Id,
            NombreComercial = t.NombreComercial,
            Ruc = t.Ruc,
            LogoUrl = t.LogoUrl,
            ColorPrimario = t.ColorPrimario,
            FaviconUrl = t.FaviconUrl
        });

        return Result.Success(tenantDtos.ToList());
    }
}