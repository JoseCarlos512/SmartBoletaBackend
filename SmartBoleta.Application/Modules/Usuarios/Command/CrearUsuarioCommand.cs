using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.Abstractions.Security;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Usuarios.Command;

public record CrearUsuarioCommand
(
    Guid TenantId,
    string Nombre,
    string Correo,
    string DNI
) : ICommand<Guid>;

internal sealed class CrearUsuarioCommandHandler : ICommandHandler<CrearUsuarioCommand, Guid>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CrearUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher
    )
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
    {

        var PasswordSalt = _passwordHasher.GenerateSalt();
        var PasswordHash = _passwordHasher.Hash("DefaultPassword123!", 
                                                PasswordSalt, 
                                                iterations: 100000, 
                                                outputBytes: 32);


        var usuario = Usuario.Create(request.TenantId,
                                     request.Nombre,
                                     request.Correo,
                                     request.DNI,
                                     PasswordHash,
                                     PasswordSalt);

        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        return Result.Success(usuario.Id);
    }
    
}

