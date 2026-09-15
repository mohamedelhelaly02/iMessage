using Api.Extensions;
using Application.Features.Auth;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        RegisterCommand command,
        CancellationToken cancellation = default)
    {
        var result = await sender.Send(command, cancellation);
        return result.ToApiResponse();
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Login(
       LoginCommand command,
       CancellationToken cancellation = default)
    {
        var result = await sender.Send(command, cancellation);
        return result.ToApiResponse();
    }

}
