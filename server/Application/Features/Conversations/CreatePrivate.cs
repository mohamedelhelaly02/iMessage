using MediatR;
using Microsoft.EntityFrameworkCore;
using server.Application.Features.Auth;
using server.Application.Interfaces;
using server.Domain.Entities;
using server.Domain.Enums;
using server.Infrastructure.Persistence;
using server.Shared.Abstraction;
using server.Shared.Extensions;
using server.Shared.Helpers;

namespace server.Application.Features.Conversations;

public sealed class CreatePrivate : IEndpoint
{
    public sealed record Command(string UserId) : IRequest<Result<string>>;

    public sealed class Handler(
        AppDbContext dbContext,
        IUser user) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            bool userExists = await dbContext.Users
                .AnyAsync(u => u.Id == request.UserId, cancellationToken);

            if (!userExists)
                return Result<string>.Failure(UserErrors.UserNotFound);

            string currentUserId = user.Id;

            if (currentUserId == request.UserId)
                return Result<string>.Failure(ConversationErrors.NotChatYourSelf);

            var existingConversationId = await dbContext.Conversations
                .AsNoTracking()
                .Where(c => c.Type == ConversationType.Private &&
                    c.Participants.Any(p => p.UserId == currentUserId) &&
                    c.Participants.Any(p => p.UserId == request.UserId))
                .Select(c => c.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!string.IsNullOrEmpty(existingConversationId))
                return Result<string>.Success(existingConversationId);

            var conversation = new Conversation()
            {
                Id = Guid.NewGuid().ToString(),
                Type = ConversationType.Private,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedById = currentUserId,
                Participants =
                [
                    new Participant
                    {
                        UserId = currentUserId,
                        Role = ParticipantRole.Admin,
                        JoinedAtUtc = DateTime.UtcNow
                    },
                    new Participant
                    {
                        UserId = request.UserId,
                        Role = ParticipantRole.Member,
                        JoinedAtUtc = DateTime.UtcNow
                    }
                ]
            };

            dbContext.Conversations.Add(conversation);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(conversation.Id);
        }
    }


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("conversations/private", async (ISender sender, Command request) =>
        {
            var res = await sender.Send(request);
            return res.ToResponse();
        })
            .WithName("CreatePrivate")
            .WithTags("Conversations")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(options => options.RequireAuthenticatedUser());
    }

}
