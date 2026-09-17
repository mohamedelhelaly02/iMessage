using Application.DTO;
using Domain.Entities;

namespace Application.Mapping;

internal static class ConversationMapping
{
    public static ConversationDto ToDto(this Conversation conversation)
    {
        return new ConversationDto(
            conversation.Id,
            conversation.ConversationType,
            conversation.Title,
            conversation.CreatedByUserId,
            conversation.CreatedAtUtc,
            conversation.LastMessageAtUtc,
            [.. conversation.Participants.Select(p =>
                new ConversationParticipantDto(
                    p.UserId,
                    p.User.DisplayName,
                    p.Role,
                    p.User.ProfilePictureUrl,
                    p.User.LastSeenAtUtc))]
            );
    }
}
