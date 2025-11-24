using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Usuarios.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Usuarios.Query;
public sealed record ObtenerUsuarioQuery (Guid UsuarioId) : IQuery<UsuarioDto>;


public class ObtenerUsuarioQueryHandler : IQueryHandler<ObtenerUsuarioQuery, UsuarioDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    public ObtenerUsuarioQueryHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }
    public async Task<Result<UsuarioDto>> Handle(ObtenerUsuarioQuery request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerUsuario(request.UsuarioId, cancellationToken);

        if (usuario is null)
        {
            return Result.Failure<UsuarioDto>(UsuariosErrors.NotFound);
        }

        var UsuarioDto = new UsuarioDto
        {
            Id = usuario.Id,
            TenantId = usuario.TenantId,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Estado = usuario.Estado
        };

        return Result.Success(UsuarioDto);
    }
}