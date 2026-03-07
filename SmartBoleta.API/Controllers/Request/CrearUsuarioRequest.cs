using SmartBoleta.Domain;

namespace SmartBoleta.API.Controllers.Request;

public record CrearUsuarioRequest(
    string Nombre,
    string Correo,
    string DNI,
    string Password,
    string Rol = Roles.User
);
