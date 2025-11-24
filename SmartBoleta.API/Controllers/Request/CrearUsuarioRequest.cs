namespace SmartBoleta.API.Controllers.Request;

public record CrearUsuarioRequest
(
    string? Nombre,
    string? Correo,
    string? DNI
);
