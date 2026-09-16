using Application.DTO;
using Application.Interfaces;
using Application.Mapping;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Conversations.CreatePrivate;

internal sealed class CreatePrivateCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService,
    IAppDbContext db)
    : IRequestHandler<CreatePrivateCommand, Result<ConversationDto>>
{
    public async Task<Result<ConversationDto>> Handle(
        CreatePrivateCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var otherUser = await userManager.FindByIdAsync(request.OtherUserId);

        if (otherUser == null)
            return Result<ConversationDto>.Failure(UserErrors.NotFound);

        var existedConversation = await db.Conversations
            .AsNoTracking()
            .Include(c => c.Participants)
            .ThenInclude(p => p.User)
            .Where(
                c => c.ConversationType == ConversationType.Private &&
                c.Participants.Count == 2 &&
                (c.Participants.Any(p => p.UserId == userId) || c.Participants.Any(p => p.UserId == request.OtherUserId)))
            .FirstOrDefaultAsync(cancellationToken);

        if (existedConversation != null)
            return Result<ConversationDto>.Success(existedConversation.ToDto());

        var conversationResult = Conversation.CreatePrivate(userId, request.OtherUserId);

        if (!conversationResult.IsSuccess)
            return Result<ConversationDto>.Failure(conversationResult.Error!);

        var conversation = conversationResult.Value!;

        db.Conversations.Add(conversation);

        await db.SaveChangesAsync(cancellationToken);

        return Result<ConversationDto>.Success(conversation.ToDto());
    }
}
