using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBoleta.API.Controllers.Request;
using SmartBoleta.Application.Modules.Usuarios.Command;
using SmartBoleta.Application.Modules.Usuarios.Query;
using SmartBoleta.Domain;

namespace SmartBoleta.API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly ISender _mediator;

    public UsuarioController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerUsuario(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObtenerUsuarioQuery(id), cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : NotFound(resultado.Error);
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> ObtenerUsuarios(CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObtenerUsuariosQuery(), cancellationToken);
        return Ok(resultado.Value);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioRequest request, CancellationToken cancellationToken)
    {
        var tenantId = ObtenerTenantId();
        if (tenantId is null) return Unauthorized();

        var command = new CrearUsuarioCommand(
            TenantId: tenantId.Value,
            Nombre: request.Nombre,
            Correo: request.Correo,
            DNI: request.DNI,
            Password: request.Password,
            Rol: request.Rol
        );

        var resultado = await _mediator.Send(command, cancellationToken);
        return resultado.IsSuccess
            ? CreatedAtAction(nameof(ObtenerUsuario), new { id = resultado.Value }, new { id = resultado.Value })
            : BadRequest(resultado.Error);
    }

    private Guid? ObtenerTenantId()
    {
        var claim = User.FindFirst("tenantId")?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
