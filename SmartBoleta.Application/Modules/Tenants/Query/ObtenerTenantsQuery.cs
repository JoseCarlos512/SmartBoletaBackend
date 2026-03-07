using Dapper;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Tenants.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Application.Modules.Tenants.Query;

public sealed record ObtenerTenantsQuery() : IQuery<List<TenantDto>>;

internal sealed class ObtenerTenantsQueryHandler : IQueryHandler<ObtenerTenantsQuery, List<TenantDto>>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ObtenerTenantsQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<List<TenantDto>>> Handle(ObtenerTenantsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var tenants = await connection.QueryAsync<TenantDto>(
            """
            SELECT TenantId   AS Id,
                   NombreComercial,
                   RUC        AS Ruc,
                   LogoUrl,
                   ColorPrimario,
                   FaviconUrl
            FROM   Tenants
            WHERE  Estado = 1
            ORDER BY NombreComercial
            """
        );

        return Result.Success(tenants.ToList());
    }
}
