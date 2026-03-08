using SmartBoleta.Domain.Enums;

namespace SmartBoleta.Application.Modules.CargaMasiva.DTOs;

public class CargaMasivaDto
{
    public Guid Id { get; set; }
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
    public List<CargaMasivaArchivoDto> Archivos { get; set; } = [];
}

public class CargaMasivaArchivoDto
{
    public Guid Id { get; set; }
    public string ArchivoNombre { get; set; } = null!;
    public CargaMasivaArchivoEstado Estado { get; set; }
    public Guid? UsuarioIdentificadoId { get; set; }
    public Guid? BoletaId { get; set; }
    public string? ErrorMensaje { get; set; }
}
