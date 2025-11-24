using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartBoleta.API.Controllers.Request;
using SmartBoleta.Application.Modules.Usuarios.Command;
using SmartBoleta.Application.Modules.Usuarios.Query;

namespace SmartBoleta.API.Controllers;

[ApiController]
[Route("api/Usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly ISender _mediator;

    public UsuarioController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerUsuario(
        Guid id, 
        CancellationToken cancellationToken
    )
    {
        var query = new ObtenerUsuarioQuery(id);
        var resultado = await _mediator.Send(query,cancellationToken);
        return resultado.IsSuccess ? Ok(resultado) : NotFound();
    }

    [HttpGet()]
    public async Task<IActionResult> ObtenerUsuarios(CancellationToken cancellationToken)
    {
        var query = new ObtenerUsuariosQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CrearUsuario(
        CrearUsuarioRequest request,
        CancellationToken cancellationToken
    )
    {
        var tenantID = Guid.Parse(HttpContext.Request.Headers["TenantID"]);
        var command = new CrearUsuarioCommand
        (
            tenantID,
            request.Nombre!,
            request.Correo!,
            request.DNI!
        );

        var resultado = await _mediator.Send(command,cancellationToken);

        if (resultado.IsSuccess)
        {
            return CreatedAtAction(nameof(ObtenerUsuarios), new { id = resultado.Value } , resultado.Value );
        }
        return BadRequest(resultado.Error);
    }

}