using Dapper;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Usuarios.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Application.Modules.Usuarios.Query;

public sealed record ObtenerUsuariosQuery() : IQuery<List<UsuarioDto>>;

internal sealed class ObtenerUsuariosQueryHandler : IQueryHandler<ObtenerUsuariosQuery, List<UsuarioDto>>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ObtenerUsuariosQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<List<UsuarioDto>>> Handle(ObtenerUsuariosQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var usuarios = await connection.QueryAsync<UsuarioDto>(
            """
            SELECT UsuarioId AS Id,
                   TenantId,
                   Nombre,
                   Correo,
                   DNI,
                   CAST(Estado AS BIT) AS Estado
            FROM   Usuarios
            WHERE  Estado = 1
            ORDER BY Nombre
            """
        );

        return Result.Success(usuarios.ToList());
    }
}
