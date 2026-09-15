using Domain.Abstractions;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public sealed class ConversationParticipant : Entity
{
    #region Properties
    public Guid UserId { get; private set; }
    public Guid ConversationId { get; private set; }
    public ParticipantRole Role { get; private set; }
    public DateTime JoinedAtUtc { get; private set; }
    #endregion

    #region Constructors
    private ConversationParticipant() { }

    private ConversationParticipant(
        Guid conversationId,
        Guid userId,
        ParticipantRole role)
    {
        Id = Guid.NewGuid();
        ConversationId = conversationId;
        UserId = userId;
        Role = role;
        JoinedAtUtc = DateTime.UtcNow;
    }
    #endregion


    #region Navigation Props
    public Conversation Conversation { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    #endregion

    #region Methods
    public static Result<ConversationParticipant> Create(
        Guid conversationId,
        Guid userId,
        ParticipantRole role)
    {
        return Result<ConversationParticipant>.Success(new ConversationParticipant(conversationId, userId, role));
    }
    #endregion

}