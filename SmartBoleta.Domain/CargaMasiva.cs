using SmartBoleta.Domain.Abstractions;
using SmartBoleta.Domain.Enums;

namespace SmartBoleta.Domain;

public class CargaMasiva : BaseEntity
{
    public CargaMasiva() { }

    private CargaMasiva(Guid id, Guid tenantId, Guid usuarioSolicitanteId, string periodo) : base(id)
    {
        TenantId = tenantId;
        UsuarioSolicitanteId = usuarioSolicitanteId;
        Periodo = periodo;
        Estado = CargaMasivaEstado.Pendiente;
        FechaInicio = DateTime.UtcNow;
    }

    public Guid TenantId { get; set; }
    public Guid UsuarioSolicitanteId { get; set; }
    public string Periodo { get; set; } = null!;
    public CargaMasivaEstado Estado { get; set; }
    public int TotalArchivos { get; set; }
    public int ArchivosProcessados { get; set; }
    public int ArchivosExitosos { get; set; }
    public int ArchivosFallidos { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public Tenant? Tenant { get; set; }
    public Usuario? UsuarioSolicitante { get; set; }
    public List<CargaMasivaArchivo> Archivos { get; set; } = [];

    public static CargaMasiva Create(Guid tenantId, Guid usuarioSolicitanteId, string periodo)
        => new(Guid.NewGuid(), tenantId, usuarioSolicitanteId, periodo);

    public void IniciarProcesamiento(int totalArchivos)
    {
        Estado = CargaMasivaEstado.Procesando;
        TotalArchivos = totalArchivos;
    }

    public void RegistrarResultadoArchivo(bool exitoso)
    {
        ArchivosProcessados++;
        if (exitoso) ArchivosExitosos++;
        else ArchivosFallidos++;
    }

    public void Completar()
    {
        Estado = ArchivosFallidos == 0
            ? CargaMasivaEstado.Completado
            : CargaMasivaEstado.CompletadoConErrores;
        FechaFin = DateTime.UtcNow;
    }
}

public static class CargaMasivaErrors
{
    public static readonly Error NotFound = new("CargaMasiva.NotFound", "No existe una carga masiva con ese ID");
    public static readonly Error AccesoDenegado = new("CargaMasiva.AccesoDenegado", "No tiene acceso a esta carga masiva");
}
