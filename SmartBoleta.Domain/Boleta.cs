using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.Enums;

namespace SmartBoleta.Domain;

public class Boleta : BaseEntity
{
    public Boleta() { }

    private Boleta(
        Guid id,
        Guid tenantId,
        Guid usuarioId,
        string periodo,
        string archivoNombre,
        string archivoUrl
    ) : base(id)
    {
        TenantId = tenantId;
        UsuarioId = usuarioId;
        Periodo = periodo;
        ArchivoNombre = archivoNombre;
        ArchivoUrl = archivoUrl;
        Estado = BoletaEstado.Pendiente;
        FechaSubida = DateTime.UtcNow;
    }

    public Guid TenantId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Periodo { get; set; } = null!;
    public string ArchivoNombre { get; set; } = null!;
    public string ArchivoUrl { get; set; } = null!;
    public BoletaEstado Estado { get; set; }
    public string? TextoOcr { get; set; }
    public DateTime FechaSubida { get; set; }
    public DateTime? FechaFirma { get; set; }

    public Tenant? Tenant { get; set; }
    public Usuario? Usuario { get; set; }

    public static Boleta Create(Guid tenantId, Guid usuarioId, string periodo, string archivoNombre, string archivoUrl)
        => new(Guid.NewGuid(), tenantId, usuarioId, periodo, archivoNombre, archivoUrl);

    public void ActualizarOcr(string textoOcr)
    {
        TextoOcr = textoOcr;
        Estado = BoletaEstado.Disponible;
    }

    public Result Firmar()
    {
        if (Estado != BoletaEstado.Disponible)
            return Result.Failure(BoletasErrors.NoDisponibleParaFirmar);

        Estado = BoletaEstado.Firmada;
        FechaFirma = DateTime.UtcNow;
        return Result.Success();
    }
}

public static class BoletasErrors
{
    public static readonly Error NotFound = new("Boletas.NotFound", "No existe una boleta con ese ID");
    public static readonly Error NoDisponibleParaFirmar = new("Boletas.NoDisponible", "La boleta no está disponible para firma. Debe estar en estado Disponible");
    public static readonly Error AccesoDenegado = new("Boletas.AccesoDenegado", "No tiene acceso a esta boleta");
}
