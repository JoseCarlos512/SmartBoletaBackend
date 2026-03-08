using SmartBoleta.Domain.Enums;

namespace SmartBoleta.Domain;

public class CargaMasivaArchivo : BaseEntity
{
    public CargaMasivaArchivo() { }

    private CargaMasivaArchivo(Guid id, Guid cargaMasivaId, string archivoNombre, string archivoUrl, string contentType) : base(id)
    {
        CargaMasivaId = cargaMasivaId;
        ArchivoNombre = archivoNombre;
        ArchivoUrl = archivoUrl;
        ContentType = contentType;
        Estado = CargaMasivaArchivoEstado.Pendiente;
    }

    public Guid CargaMasivaId { get; set; }
    public string ArchivoNombre { get; set; } = null!;
    public string ArchivoUrl { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public CargaMasivaArchivoEstado Estado { get; set; }
    public Guid? UsuarioIdentificadoId { get; set; }
    public Guid? BoletaId { get; set; }
    public string? ErrorMensaje { get; set; }
    public string? TextoOcr { get; set; }

    public CargaMasiva? CargaMasiva { get; set; }

    public static CargaMasivaArchivo Create(Guid cargaMasivaId, string archivoNombre, string archivoUrl, string contentType)
        => new(Guid.NewGuid(), cargaMasivaId, archivoNombre, archivoUrl, contentType);

    public void MarcarProcesando() => Estado = CargaMasivaArchivoEstado.Procesando;

    public void MarcarExitoso(Guid usuarioId, Guid boletaId, string textoOcr)
    {
        Estado = CargaMasivaArchivoEstado.Exitoso;
        UsuarioIdentificadoId = usuarioId;
        BoletaId = boletaId;
        TextoOcr = textoOcr;
    }

    public void MarcarFallido(string errorMensaje)
    {
        Estado = CargaMasivaArchivoEstado.Fallido;
        ErrorMensaje = errorMensaje;
    }
}
