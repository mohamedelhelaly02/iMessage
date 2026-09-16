using Application.DTO;
using Application.Interfaces;
using Application.Mapping;
using Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversations.GetConversations;

internal sealed class GetConversationsQueryHandler(
    IAppDbContext db,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetConversationsQuery, Result<List<ConversationDto>>>
{
    public async Task<Result<List<ConversationDto>>> Handle(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var conversations = await db.Conversations
            .AsNoTracking()
            .Include(c => c.Participants)
            .ThenInclude(p => p.User)
            .Where(c =>
                c.CreatedByUserId == userId ||
                c.Participants.Any(p => p.UserId == userId))
            .Select(c => c.ToDto())
            .ToListAsync(cancellationToken);

        return Result<List<ConversationDto>>.Success(conversations);
    }
}
