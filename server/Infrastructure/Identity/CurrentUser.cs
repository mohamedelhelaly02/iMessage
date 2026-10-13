using System.Security.Claims;
using server.Application.Interfaces;

namespace server.Infrastructure.Identity;

internal sealed class CurrentUser(IHttpContextAccessor accessor) : IUser
{
    public string Id => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
}
