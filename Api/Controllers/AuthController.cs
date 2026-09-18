using Api.Extensions;
using Application.DTO;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        [FromServices] IValidator<RegisterCommand> validator,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command,
            cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToValidationApiResponse();

        var result = await sender.Send(command, cancellationToken);
        return result.ToApiResponse();
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
       [FromBody] LoginCommand command,
       [FromServices] IValidator<LoginCommand> validator,
       CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
                 command,
                 cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToValidationApiResponse();

        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResponse();
    }

}
