using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Tenants.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Tenants.Query;
public sealed record ObtenerTenantQuery (Guid TenantId) : IQuery<TenantDto>;


public class ObtenerTenantQueryHandler : IQueryHandler<ObtenerTenantQuery, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    public ObtenerTenantQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }
    public async Task<Result<TenantDto>> Handle(ObtenerTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.ObtenerTenant(request.TenantId, cancellationToken);

        if (tenant is null)
        {
            return Result.Failure<TenantDto>(TenantsErrors.NotFound);
        }

        var tenantDto = new TenantDto
        {
            Id = tenant.Id,
            NombreComercial = tenant.NombreComercial,
            Ruc = tenant.Ruc,
            LogoUrl = tenant.LogoUrl,
            ColorPrimario = tenant.ColorPrimario,
            FaviconUrl = tenant.FaviconUrl
        };

        return Result.Success(tenantDto);
    }
}