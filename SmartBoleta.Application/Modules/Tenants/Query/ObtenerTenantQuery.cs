using Dapper;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Tenants.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Application.Modules.Tenants.Query;

public sealed record ObtenerTenantQuery(Guid TenantId) : IQuery<TenantDto>;

internal sealed class ObtenerTenantQueryHandler : IQueryHandler<ObtenerTenantQuery, TenantDto>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ObtenerTenantQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<TenantDto>> Handle(ObtenerTenantQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var tenant = await connection.QueryFirstOrDefaultAsync<TenantDto>(
            """
            SELECT TenantId   AS Id,
                   NombreComercial,
                   RUC        AS Ruc,
                   LogoUrl,
                   ColorPrimario,
                   FaviconUrl
            FROM   Tenants
            WHERE  TenantId = @TenantId
              AND  Estado    = 1
            """,
            new { request.TenantId }
        );

        if (tenant is null)
            return Result.Failure<TenantDto>(TenantsErrors.NotFound);

        return Result.Success(tenant);
    }
}
