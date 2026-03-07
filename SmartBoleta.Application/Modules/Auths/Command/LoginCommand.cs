using FluentValidation;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Application.Modules.Auths.DTOs;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.Abstractions.Security;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Auths.Command;

public record LoginCommand(string Correo, string Password) : ICommand<LoginResultDto>;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService
    )
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<LoginResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerUsuarioPorCorreo(request.Correo, cancellationToken);
        if (usuario is null)
            return Result.Failure<LoginResultDto>(UsuariosErrors.InvalidCredentials);

        var passwordValido = _passwordHasher.Verify(request.Password, usuario.PasswordSalt, usuario.PasswordHash);
        if (!passwordValido)
            return Result.Failure<LoginResultDto>(UsuariosErrors.InvalidCredentials);

        var token = _jwtTokenService.GenerateToken(
            usuario.Id,
            usuario.TenantId,
            usuario.Correo ?? string.Empty,
            [usuario.Rol],
            out DateTime expiresAt
        );

        return Result.Success(new LoginResultDto(
            Token: token,
            ExpiresAt: expiresAt,
            UsuarioId: usuario.Id,
            TenantId: usuario.TenantId,
            Nombre: usuario.Nombre,
            Correo: usuario.Correo ?? string.Empty,
            Rol: usuario.Rol
        ));
    }
}

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Correo).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
