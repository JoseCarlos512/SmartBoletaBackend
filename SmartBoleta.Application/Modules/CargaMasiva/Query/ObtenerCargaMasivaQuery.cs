using Dapper;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.CargaMasiva.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Application.Modules.CargaMasiva.Query;

public sealed record ObtenerCargaMasivaQuery(Guid CargaMasivaId, Guid TenantId) : IQuery<CargaMasivaDto>;

internal sealed class ObtenerCargaMasivaQueryHandler : IQueryHandler<ObtenerCargaMasivaQuery, CargaMasivaDto>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ObtenerCargaMasivaQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<CargaMasivaDto>> Handle(ObtenerCargaMasivaQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var cargaMasivaDic = new Dictionary<Guid, CargaMasivaDto>();

        await connection.QueryAsync<CargaMasivaDto, CargaMasivaArchivoDto, CargaMasivaDto>(
            """
            SELECT cm.CargaMasivaId        AS Id,
                   cm.TenantId,
                   cm.UsuarioSolicitanteId,
                   cm.Periodo,
                   cm.Estado,
                   cm.TotalArchivos,
                   cm.ArchivosProcessados,
                   cm.ArchivosExitosos,
                   cm.ArchivosFallidos,
                   cm.FechaInicio,
                   cm.FechaFin,
                   cma.CargaMasivaArchivoId AS Id,
                   cma.ArchivoNombre,
                   cma.Estado,
                   cma.UsuarioIdentificadoId,
                   cma.BoletaId,
                   cma.ErrorMensaje
            FROM   CargaMasivas cm
            LEFT JOIN CargaMasivaArchivos cma ON cma.CargaMasivaId = cm.CargaMasivaId
            WHERE  cm.CargaMasivaId = @CargaMasivaId
              AND  cm.TenantId      = @TenantId
            """,
            (cm, archivo) =>
            {
                if (!cargaMasivaDic.TryGetValue(cm.Id, out var entry))
                {
                    entry = cm;
                    cargaMasivaDic[cm.Id] = entry;
                }
                if (archivo is not null)
                    entry.Archivos.Add(archivo);
                return entry;
            },
            new { request.CargaMasivaId, request.TenantId },
            splitOn: "Id"
        );

        if (!cargaMasivaDic.TryGetValue(request.CargaMasivaId, out var resultado))
            return Result.Failure<CargaMasivaDto>(CargaMasivaErrors.NotFound);

        return Result.Success(resultado);
    }
}
