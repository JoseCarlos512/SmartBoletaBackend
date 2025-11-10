using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartBoleta.API.Controllers.Request;
using SmartBoleta.Application.Modules.Tenants.Command;
using SmartBoleta.Application.Modules.Tenants.Query;

namespace SmartBoleta.API.Controllers;

[ApiController]
[Route("api/tenants")]
public class TenantController : ControllerBase
{
    private readonly ISender _mediator;

    public TenantController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerTenant(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var query = new ObtenerTenantQuery(id);
        var resultado = await _mediator.Send(query,cancellationToken);
        return resultado.IsSuccess ? Ok(resultado) : NotFound();
    }


    [HttpGet()]
    public async Task<IActionResult> ObtenerTenants(CancellationToken cancellationToken)
    {
        var query = new ObtenerTenantsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CrearTenant(
        CrearTenantRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CrearTenantCommand
        (
            request.NombreComercial!,
            request.Ruc!,
            request.LogoUrl!,
            request.ColorPrimario!,
            request.FaviconUrl!
        );

        var resultado = await _mediator.Send(command,cancellationToken);

        if (resultado.IsSuccess)
        {
            return CreatedAtAction(nameof(ObtenerTenants), new { id = resultado.Value } , resultado.Value );
        }
        return BadRequest(resultado.Error);
    }
}
