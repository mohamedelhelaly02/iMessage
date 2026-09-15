using Application.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversations.CreatePrivate;

internal sealed class CreatePrivateCommandHandler(
    ICurrentUserService currentUserService,
    IAppDbContext db)
    : IRequestHandler<CreatePrivateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        CreatePrivateCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var existedConversation = await db.Conversations
            .AsNoTracking()
            .Where(
                c => c.ConversationType == ConversationType.Private &&
                c.Participants.Count == 2 &&
                (c.Participants.Any(p => p.UserId == userId) || c.Participants.Any(p => p.UserId == request.OtherUserId)))
            .FirstOrDefaultAsync(cancellationToken);

        if (existedConversation != null)
            return Result<string>.Success(existedConversation.Id);

        var conversationResult = Conversation.CreatePrivate(userId, request.OtherUserId);

        if (!conversationResult.IsSuccess)
            return Result<string>.Failure(conversationResult.Error!);

        var conversation = conversationResult.Value!;

        db.Conversations.Add(conversation);

        await db.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(conversation.Id);
    }
}
