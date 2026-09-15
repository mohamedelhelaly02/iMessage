using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
internal sealed class ChatHub(ILogger<ChatHub> logger) : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        logger.LogInformation("User connected: {UserId}", userId);

        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        logger.LogInformation("User disconnected: {UserId}", userId);
        return Task.CompletedTask;
    }

    public async Task NotifyCallerOnline()
    {
        await Clients.Caller.SendAsync("ReceiveCallerConnected");
    }

}
