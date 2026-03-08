using FluentValidation;
using SmartBoleta.Application.Abstractions.Messaging;
using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.IRepositories;
using CargaMasivaEntity = SmartBoleta.Domain.CargaMasiva;
using CargaMasivaArchivoEntity = SmartBoleta.Domain.CargaMasivaArchivo;

namespace SmartBoleta.Application.Modules.Boletas.Command;

public record SubirBoletasMasivasCommand(
    Guid TenantId,
    Guid UsuarioSolicitanteId,
    string Periodo,
    List<ArchivoMasivo> Archivos
) : ICommand<Guid>;

public record ArchivoMasivo(string Nombre, byte[] Contenido, string ContentType);

internal sealed class SubirBoletasMasivasCommandHandler : ICommandHandler<SubirBoletasMasivasCommand, Guid>
{
    private readonly ICargaMasivaRepository _cargaMasivaRepository;
    private readonly IStorageService _storageService;
    private readonly IJobScheduler _jobScheduler;

    public SubirBoletasMasivasCommandHandler(
        ICargaMasivaRepository cargaMasivaRepository,
        IStorageService storageService,
        IJobScheduler jobScheduler)
    {
        _cargaMasivaRepository = cargaMasivaRepository;
        _storageService = storageService;
        _jobScheduler = jobScheduler;
    }

    public async Task<Result<Guid>> Handle(SubirBoletasMasivasCommand request, CancellationToken cancellationToken)
    {
        var cargaMasiva = CargaMasivaEntity.Create(request.TenantId, request.UsuarioSolicitanteId, request.Periodo);

        foreach (var archivo in request.Archivos)
        {
            using var stream = new MemoryStream(archivo.Contenido);
            var archivoUrl = await _storageService.SubirArchivoAsync(
                stream, archivo.Nombre, archivo.ContentType, cancellationToken);

            cargaMasiva.Archivos.Add(
                CargaMasivaArchivoEntity.Create(cargaMasiva.Id, archivo.Nombre, archivoUrl, archivo.ContentType)
            );
        }

        await _cargaMasivaRepository.AddAsync(cargaMasiva, cancellationToken);

        _jobScheduler.EnqueueCargaMasivaJob(cargaMasiva.Id);

        return Result.Success(cargaMasiva.Id);
    }
}

internal sealed class SubirBoletasMasivasCommandValidator : AbstractValidator<SubirBoletasMasivasCommand>
{
    public SubirBoletasMasivasCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.UsuarioSolicitanteId).NotEmpty();
        RuleFor(x => x.Periodo)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{2}$")
            .WithMessage("El periodo debe tener formato YYYY-MM (ej. 2024-01)");
        RuleFor(x => x.Archivos)
            .NotNull()
            .Must(a => a.Count > 0).WithMessage("Debe enviar al menos un archivo")
            .Must(a => a.Count <= 100).WithMessage("No puede enviar más de 100 archivos por carga");
        RuleForEach(x => x.Archivos).ChildRules(archivo =>
        {
            archivo.RuleFor(a => a.Nombre).NotEmpty().MaximumLength(255);
            archivo.RuleFor(a => a.Contenido).NotNull().Must(c => c.Length > 0).WithMessage("El archivo no puede estar vacío");
            archivo.RuleFor(a => a.ContentType).NotEmpty();
        });
    }
}
