using System;

namespace SmartBoleta.Application.Modules.Auths.DTOs;

public record LoginResultDto
(
    string Token,
    Guid UsuarioId,
    string Nombre,
    string Correo
);