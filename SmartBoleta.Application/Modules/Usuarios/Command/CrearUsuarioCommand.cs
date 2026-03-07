using FluentValidation;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.Abstractions.Security;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Usuarios.Command;

public record CrearUsuarioCommand(
    Guid TenantId,
    string Nombre,
    string Correo,
    string DNI,
    string Password,
    string Rol = Roles.User
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
        var salt = _passwordHasher.GenerateSalt();
        var hash = _passwordHasher.Hash(request.Password, salt);

        var usuario = Usuario.Create(
            request.TenantId,
            request.Nombre,
            request.Correo,
            request.DNI,
            request.Rol,
            hash,
            salt
        );

        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        return Result.Success(usuario.Id);
    }
}

internal sealed class CrearUsuarioCommandValidator : AbstractValidator<CrearUsuarioCommand>
{
    public CrearUsuarioCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.DNI).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres");
        RuleFor(x => x.Rol).NotEmpty().Must(r => r == Roles.Admin || r == Roles.Manager || r == Roles.User)
            .WithMessage($"El rol debe ser {Roles.Admin}, {Roles.Manager} o {Roles.User}");
    }
}
