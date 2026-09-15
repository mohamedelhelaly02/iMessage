using Api.Extensions;
using Application.Features.Conversations.CreatePrivate;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class ConversationsController(ISender sender) : ControllerBase
{
    [HttpPost("private")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePrivate(
        [FromBody] CreatePrivateCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResponse();
    }
}