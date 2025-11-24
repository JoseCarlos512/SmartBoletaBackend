using System;

namespace SmartBoleta.API.Controllers.Request;

public record LoginRequest
(
    string Correo,
    string Password
);