using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBoleta.API.Controllers.Request;
using SmartBoleta.Application.Modules.Boletas.Command;
using SmartBoleta.Application.Modules.Boletas.Query;

namespace SmartBoleta.API.Controllers;

[ApiController]
[Route("api/boletas")]
[Authorize]
public class BoletaController : ControllerBase
{
    private readonly ISender _mediator;

    public BoletaController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerBoleta(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObtenerBoletaQuery(id), cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : NotFound(resultado.Error);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> ObtenerBoletasPorUsuario(Guid usuarioId, CancellationToken cancellationToken)
    {
        var tenantId = ObtenerTenantId();
        if (tenantId is null) return Unauthorized();

        var resultado = await _mediator.Send(new ObtenerBoletasPorUsuarioQuery(usuarioId, tenantId.Value), cancellationToken);
        return Ok(resultado.Value);
    }

    [HttpGet("tenant")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ObtenerBoletasPorTenant(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default
    )
    {
        var tenantId = ObtenerTenantId();
        if (tenantId is null) return Unauthorized();

        var resultado = await _mediator.Send(new ObtenerBoletasPorTenantQuery(tenantId.Value, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50 MB
    public async Task<IActionResult> SubirBoleta([FromForm] SubirBoletaRequest request, CancellationToken cancellationToken)
    {
        var tenantId = ObtenerTenantId();
        if (tenantId is null) return Unauthorized();

        using var ms = new MemoryStream();
        await request.Archivo.CopyToAsync(ms, cancellationToken);

        var command = new SubirBoletaCommand(
            TenantId: tenantId.Value,
            UsuarioId: request.UsuarioId,
            Periodo: request.Periodo,
            ArchivoNombre: request.Archivo.FileName,
            ContenidoArchivo: ms.ToArray(),
            ContentType: request.Archivo.ContentType
        );

        var resultado = await _mediator.Send(command, cancellationToken);
        return resultado.IsSuccess
            ? CreatedAtAction(nameof(ObtenerBoleta), new { id = resultado.Value }, new { id = resultado.Value })
            : BadRequest(resultado.Error);
    }

    [HttpPut("{id}/firmar")]
    public async Task<IActionResult> FirmarBoleta(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = ObtenerUsuarioId();
        if (usuarioId is null) return Unauthorized();

        var resultado = await _mediator.Send(new FirmarBoletaCommand(id, usuarioId.Value), cancellationToken);
        if (resultado.IsSuccess) return NoContent();

        return resultado.Error.Code == "Boletas.AccesoDenegado" ? Forbid() : BadRequest(resultado.Error);
    }

    private Guid? ObtenerTenantId()
    {
        var claim = User.FindFirst("tenantId")?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    private Guid? ObtenerUsuarioId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
