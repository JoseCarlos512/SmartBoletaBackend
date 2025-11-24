
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBoleta.API.Controllers.Request;
using SmartBoleta.Application.Modules.Auths.Command;

namespace SmartBoleta.API.Controllers;

[ApiController]
[Route("api/Auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;
    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginRequest req,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginCommand(
            req.Correo, 
            req.Password
        );
        
        var resultado = await _mediator.Send(command, cancellationToken);

        if (resultado.IsSuccess)
        {
            return Ok(resultado.Value);
        }
        return Unauthorized();
    }
}
