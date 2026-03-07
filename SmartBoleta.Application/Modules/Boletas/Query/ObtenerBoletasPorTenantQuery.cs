using Dapper;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Boletas.DTOs;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Application.Modules.Boletas.Query;

public sealed record ObtenerBoletasPorTenantQuery(Guid TenantId, int Pagina = 1, int TamanoPagina = 20) : IQuery<List<BoletaDto>>;

internal sealed class ObtenerBoletasPorTenantQueryHandler : IQueryHandler<ObtenerBoletasPorTenantQuery, List<BoletaDto>>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ObtenerBoletasPorTenantQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<List<BoletaDto>>> Handle(ObtenerBoletasPorTenantQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var offset = (request.Pagina - 1) * request.TamanoPagina;

        var boletas = await connection.QueryAsync<BoletaDto>(
            """
            SELECT BoletaId    AS Id,
                   TenantId,
                   UsuarioId,
                   Periodo,
                   ArchivoNombre,
                   ArchivoUrl,
                   Estado,
                   TextoOcr,
                   FechaSubida,
                   FechaFirma
            FROM   Boletas
            WHERE  TenantId = @TenantId
            ORDER BY FechaSubida DESC
            OFFSET @Offset ROWS FETCH NEXT @TamanoPagina ROWS ONLY
            """,
            new { request.TenantId, Offset = offset, request.TamanoPagina }
        );

        return Result.Success(boletas.ToList());
    }
}
