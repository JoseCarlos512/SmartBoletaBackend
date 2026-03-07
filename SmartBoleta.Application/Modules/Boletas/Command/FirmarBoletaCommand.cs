using FluentValidation;
using SmartBoleta.Application.Abstractions;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Boletas.Command;

public record FirmarBoletaCommand(Guid BoletaId, Guid UsuarioId) : ICommand;

internal sealed class FirmarBoletaCommandHandler : ICommandHandler<FirmarBoletaCommand>
{
    private readonly IBoletaRepository _boletaRepository;
    private readonly INotificationService _notificationService;

    public FirmarBoletaCommandHandler(
        IBoletaRepository boletaRepository,
        INotificationService notificationService
    )
    {
        _boletaRepository = boletaRepository;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(FirmarBoletaCommand request, CancellationToken cancellationToken)
    {
        var boleta = await _boletaRepository.ObtenerPorId(request.BoletaId, cancellationToken);
        if (boleta is null)
            return Result.Failure(BoletasErrors.NotFound);

        if (boleta.UsuarioId != request.UsuarioId)
            return Result.Failure(BoletasErrors.AccesoDenegado);

        var resultado = boleta.Firmar();
        if (resultado.IsFailure)
            return resultado;

        await _boletaRepository.UpdateAsync(boleta, cancellationToken);

        await _notificationService.NotificarUsuarioAsync(
            request.UsuarioId,
            "boleta_firmada",
            new { boleta.Id, boleta.Periodo },
            cancellationToken
        );

        return Result.Success();
    }
}

internal sealed class FirmarBoletaCommandValidator : AbstractValidator<FirmarBoletaCommand>
{
    public FirmarBoletaCommandValidator()
    {
        RuleFor(x => x.BoletaId).NotEmpty();
        RuleFor(x => x.UsuarioId).NotEmpty();
    }
}
