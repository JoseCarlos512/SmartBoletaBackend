namespace SmartBoleta.API.Controllers.Request;

public record SubirBoletaRequest(
    Guid UsuarioId,
    string Periodo,
    IFormFile Archivo
);
