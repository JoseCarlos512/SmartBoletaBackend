using Dapper;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Boletas.DTOs;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Application.Modules.Boletas.Query;

public sealed record ObtenerBoletasPorUsuarioQuery(Guid UsuarioId, Guid TenantId) : IQuery<List<BoletaDto>>;

internal sealed class ObtenerBoletasPorUsuarioQueryHandler : IQueryHandler<ObtenerBoletasPorUsuarioQuery, List<BoletaDto>>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ObtenerBoletasPorUsuarioQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<List<BoletaDto>>> Handle(ObtenerBoletasPorUsuarioQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

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
            WHERE  UsuarioId = @UsuarioId
              AND  TenantId  = @TenantId
            ORDER BY FechaSubida DESC
            """,
            new { request.UsuarioId, request.TenantId }
        );

        return Result.Success(boletas.ToList());
    }
}
