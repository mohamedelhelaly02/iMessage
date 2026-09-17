using Api.Hubs.Helpers;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
internal sealed class ChatHub(
    IAppDbContext db,
    ILogger<ChatHub> logger,
    ICurrentUserService currentUserService,
    IConnectionTracker connectionTracker) : Hub
{
    private const string GroupNamePrefix = "user:";
    public override async Task OnConnectedAsync()
    {
        var userId = currentUserService.GetUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(userId));

        var isFirstConnection = connectionTracker.AddConnection(userId, Context.ConnectionId);

        logger.LogInformation(
            "User connected: {UserId}, Connection: {ConnectionId}",
            userId,
            Context.ConnectionId);

        if (isFirstConnection)
        {
            await NotifyRelatedUsersStatus(userId, true);
        }

        await base.OnConnectedAsync();
    }

    private async Task NotifyRelatedUsersStatus(string userId, bool isOnline)
    {
        var conversationIds = await db.ConversationParticipants
         .AsNoTracking()
         .Where(p => p.UserId == userId)
         .Select(p => p.ConversationId)
         .ToListAsync();

        if (conversationIds.Count == 0)
            return;

        var relatedUserIds = await db.ConversationParticipants
            .AsNoTracking()
            .Where(p => conversationIds.Contains(p.ConversationId) && p.UserId != userId)
            .Select(p => p.UserId)
            .Distinct()
            .ToListAsync();

        if (relatedUserIds?.Count > 0)
        {
            var eventName = isOnline ? "UserOnline" : "UserOffline";
            foreach (var user in relatedUserIds)
                await Clients.Group(GetGroupName(user)).SendAsync(eventName, userId);
        }

    }

    private static string GetGroupName(string userId)
        => $"{GroupNamePrefix}{userId}";

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = currentUserService.GetUserId();
        var isLastConnection = connectionTracker.RemoveConnection(
            userId,
            Context.ConnectionId);

        logger.LogInformation(
            "User disconnected: {UserId}, Connection: {ConnectionId}",
            userId,
            Context.ConnectionId);

        if (isLastConnection)
        {
            await NotifyRelatedUsersStatus(userId, false);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task NotifyCallerOnline()
    {
        await Clients.Caller.SendAsync("ReceiveCallerConnected");
    }

    public async Task GetOnlineStatus(IEnumerable<string> userIds)
    {
        var userId = currentUserService.GetUserId();

        var onlineStatus = userIds.Distinct()
            .ToDictionary(id => id, connectionTracker.IsOnline);

        await Clients.User(userId).SendAsync("OnlineStatus", onlineStatus);
    }

}
