
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Usuarios.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Usuarios.Query;

public sealed record ObtenerUsuariosQuery() : IQuery<List<UsuarioDto>>;

public class ObtenerUsuariosQueryHandler : IQueryHandler<ObtenerUsuariosQuery, List<UsuarioDto>>
{
    private readonly IUsuarioRepository _usuarioRepository;
    public ObtenerUsuariosQueryHandler(IUsuarioRepository TenantRepository)
    {
        _usuarioRepository = TenantRepository;
    }
    public async Task<Result<List<UsuarioDto>>> Handle(ObtenerUsuariosQuery request, CancellationToken cancellationToken)
    {

        var Usuarios = await _usuarioRepository.ObtenerUsuarios(cancellationToken);

        if (Usuarios is null || !Usuarios.Any())
        {
            return Result.Failure<List<UsuarioDto>>(UsuariosErrors.NotFound);
        }

        var Usuariotos = Usuarios.Select(t => new UsuarioDto
        {
            Id = t.Id,
            TenantId = t.TenantId,
            Nombre = t.Nombre,
            Correo = t.Correo,
            Estado = t.Estado
        });

        return Result.Success(Usuariotos.ToList());
    }
}