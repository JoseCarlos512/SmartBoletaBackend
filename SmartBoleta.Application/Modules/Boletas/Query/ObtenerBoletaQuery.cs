using Dapper;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Boletas.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Application.Modules.Boletas.Query;

public sealed record ObtenerBoletaQuery(Guid BoletaId) : IQuery<BoletaDto>;

internal sealed class ObtenerBoletaQueryHandler : IQueryHandler<ObtenerBoletaQuery, BoletaDto>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ObtenerBoletaQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<BoletaDto>> Handle(ObtenerBoletaQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var boleta = await connection.QueryFirstOrDefaultAsync<BoletaDto>(
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
            WHERE  BoletaId = @BoletaId
            """,
            new { request.BoletaId }
        );

        if (boleta is null)
            return Result.Failure<BoletaDto>(BoletasErrors.NotFound);

        return Result.Success(boleta);
    }
}
