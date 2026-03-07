using FluentValidation;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.IRepositories;

namespace SmartBoleta.Application.Modules.Boletas.Command;

public record SubirBoletaCommand(
    Guid TenantId,
    Guid UsuarioId,
    string Periodo,
    string ArchivoNombre,
    byte[] ContenidoArchivo,
    string ContentType
) : ICommand<Guid>;

internal sealed class SubirBoletaCommandHandler : ICommandHandler<SubirBoletaCommand, Guid>
{
    private readonly IBoletaRepository _boletaRepository;
    private readonly IStorageService _storageService;
    private readonly IJobScheduler _jobScheduler;

    public SubirBoletaCommandHandler(
        IBoletaRepository boletaRepository,
        IStorageService storageService,
        IJobScheduler jobScheduler
    )
    {
        _boletaRepository = boletaRepository;
        _storageService = storageService;
        _jobScheduler = jobScheduler;
    }

    public async Task<Result<Guid>> Handle(SubirBoletaCommand request, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(request.ContenidoArchivo);
        var archivoUrl = await _storageService.SubirArchivoAsync(
            stream,
            request.ArchivoNombre,
            request.ContentType,
            cancellationToken
        );

        var boleta = Boleta.Create(
            request.TenantId,
            request.UsuarioId,
            request.Periodo,
            request.ArchivoNombre,
            archivoUrl
        );

        await _boletaRepository.AddAsync(boleta, cancellationToken);

        _jobScheduler.EnqueueOcrJob(boleta.Id);

        return Result.Success(boleta.Id);
    }
}

internal sealed class SubirBoletaCommandValidator : AbstractValidator<SubirBoletaCommand>
{
    public SubirBoletaCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.UsuarioId).NotEmpty();
        RuleFor(x => x.Periodo)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{2}$")
            .WithMessage("El periodo debe tener formato YYYY-MM (ej. 2024-01)");
        RuleFor(x => x.ArchivoNombre).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContenidoArchivo).NotNull().Must(c => c.Length > 0).WithMessage("El archivo no puede estar vacío");
        RuleFor(x => x.ContentType).NotEmpty();
    }
}
