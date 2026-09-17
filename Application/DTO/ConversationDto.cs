using Domain.Enums;

namespace Application.DTO;

public sealed record ConversationParticipantDto(
    string UserId,
    string DisplayName,
    ParticipantRole role,
    string? PictureUrl,
    DateTime? LastSeenAtUtc);

public sealed record ConversationDto(
    string Id,
    ConversationType ConversationType,
    string? Title,
    string CreatedByUserId,
    DateTime CreatedAtUtc,
    DateTime? LastMessageAtUtc,
    ConversationParticipantDto[] Participants);