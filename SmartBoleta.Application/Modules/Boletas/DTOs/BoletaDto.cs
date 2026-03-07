using SmartBoleta.Domain.Enums;

namespace SmartBoleta.Application.Modules.Boletas.DTOs;

public class BoletaDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Periodo { get; set; } = null!;
    public string ArchivoNombre { get; set; } = null!;
    public string ArchivoUrl { get; set; } = null!;
    public BoletaEstado Estado { get; set; }
    public string? TextoOcr { get; set; }
    public DateTime FechaSubida { get; set; }
    public DateTime? FechaFirma { get; set; }
}
