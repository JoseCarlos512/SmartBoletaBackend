using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBoleta.API.Controllers.Request;
using SmartBoleta.Application.Modules.Tenants.Command;
using SmartBoleta.Application.Modules.Tenants.Query;
using SmartBoleta.Domain;

namespace SmartBoleta.API.Controllers;

[ApiController]
[Route("api/tenants")]
[Authorize(Roles = Roles.Admin)]
public class TenantController : ControllerBase
{
    private readonly ISender _mediator;

    public TenantController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerTenant(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObtenerTenantQuery(id), cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : NotFound(resultado.Error);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTenants(CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObtenerTenantsQuery(), cancellationToken);
        return Ok(resultado.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CrearTenant([FromBody] CrearTenantRequest request, CancellationToken cancellationToken)
    {
        var command = new CrearTenantCommand(
            request.NombreComercial!,
            request.Ruc!,
            request.LogoUrl!,
            request.ColorPrimario!,
            request.FaviconUrl!
        );

        var resultado = await _mediator.Send(command, cancellationToken);
        return resultado.IsSuccess
            ? CreatedAtAction(nameof(ObtenerTenant), new { id = resultado.Value }, new { id = resultado.Value })
            : BadRequest(resultado.Error);
    }
}
