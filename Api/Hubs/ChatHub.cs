using Api.Hubs.Helpers;
using Application.DTO;
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
        try
        {
            var userId = currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(userId));

            var isFirstConnection = connectionTracker.AddConnection(userId, Context.ConnectionId);

            logger.LogInformation(
                "User connected: {UserId}, Connection: {ConnectionId}",
                userId,
                Context.ConnectionId);

            if (isFirstConnection)
                await NotifyRelatedUsersStatus(userId, true);

        }
        finally
        {
            await base.OnConnectedAsync();
        }

    }

    private async Task NotifyRelatedUsersStatus(string userId, bool isOnline)
    {
        var user = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserDto(
                u.Id,
                u.DisplayName,
                u.UserName,
                u.Email,
                u.ProfilePictureUrl,
                u.LastSeenAtUtc))
            .FirstOrDefaultAsync();

        if (user is null)
            return;

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

        var eventName = isOnline ? "UserOnline" : "UserOffline";

        if (relatedUserIds?.Count > 0)
        {
            foreach (var id in relatedUserIds)
                await Clients.Group(GetGroupName(id)).SendAsync(eventName, user);
        }

    }

    private static string GetGroupName(string userId)
        => $"{GroupNamePrefix}{userId}";

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = currentUserService.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var isLastConnection = connectionTracker.RemoveConnection(
                userId,
                Context.ConnectionId);

            logger.LogInformation(
                "User disconnected: {UserId}, Connection: {ConnectionId}",
                userId,
                Context.ConnectionId);

            if (!isLastConnection)
            {
                return;
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user is not null)
            {
                user.MarkAsSeen();
                await db.SaveChangesAsync(CancellationToken.None);
            }

            await NotifyRelatedUsersStatus(userId, isOnline: false);
        }
        finally
        {
            await base.OnDisconnectedAsync(exception);
        }
    }

    public async Task NotifyCallerOnline()
    {
        await Clients.Caller.SendAsync("ReceiveCallerConnected");
    }

    public async Task GetOnlineStatus(IEnumerable<string> userIds)
    {
        var requestedUserIds = userIds?
               .Where(id => !string.IsNullOrWhiteSpace(id))
               .Distinct()
               .ToList();

        if (requestedUserIds is null || requestedUserIds.Count == 0)
        {
            await Clients.Caller.SendAsync(
                "OnlineStatus",
                Array.Empty<UserPresenceDto>());

            return;
        }

        var users = await db.Users
            .AsNoTracking()
            .Where(user => requestedUserIds.Contains(user.Id))
            .Select(user => new UserDto(
                user.Id,
                user.DisplayName,
                user.UserName!,
                user.Email!,
                user.ProfilePictureUrl,
                user.LastSeenAtUtc))
            .ToListAsync();

        var onlineStatuses = users
            .Select(user => new UserPresenceDto(
                user,
                connectionTracker.IsOnline(user.Id)))
            .ToList();

        await Clients.Caller.SendAsync("OnlineStatus", onlineStatuses);
    }

}
