using MediatR;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet()]
    public async Task<IActionResult> ObtenerTenants(CancellationToken cancellationToken)
    {
        var query = new ObtenerTenantsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
