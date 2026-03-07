using Dapper;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Usuarios.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Application.Modules.Usuarios.Query;

public sealed record ObtenerUsuarioQuery(Guid UsuarioId) : IQuery<UsuarioDto>;

internal sealed class ObtenerUsuarioQueryHandler : IQueryHandler<ObtenerUsuarioQuery, UsuarioDto>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ObtenerUsuarioQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<UsuarioDto>> Handle(ObtenerUsuarioQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var usuario = await connection.QueryFirstOrDefaultAsync<UsuarioDto>(
            """
            SELECT UsuarioId AS Id,
                   TenantId,
                   Nombre,
                   Correo,
                   DNI,
                   CAST(Estado AS BIT) AS Estado
            FROM   Usuarios
            WHERE  UsuarioId = @UsuarioId
              AND  Estado    = 1
            """,
            new { request.UsuarioId }
        );

        if (usuario is null)
            return Result.Failure<UsuarioDto>(UsuariosErrors.NotFound);

        return Result.Success(usuario);
    }
}
