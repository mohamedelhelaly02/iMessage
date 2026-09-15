using Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Identity;

internal sealed class CurrentUserService(
    IHttpContextAccessor accessor) : ICurrentUserService
{
    public string GetUserId()
    {
        var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        return userId;
    }
}
