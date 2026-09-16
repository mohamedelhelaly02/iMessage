using Domain.Abstractions;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public sealed class Conversation : Entity
{
    #region Properties
    private readonly HashSet<ConversationParticipant> _participants = [];

    public ConversationType ConversationType { get; private set; }
    public string? Title { get; private set; }
    public string CreatedByUserId { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? LastMessageAtUtc { get; private set; }
    #endregion

    #region Constructors
    private Conversation() { }

    private Conversation(
        ConversationType type,
        string? title,
        string createdByUserId)
    {
        Id = Guid.NewGuid().ToString();
        ConversationType = type;
        CreatedByUserId = createdByUserId;
        Title = title;
        CreatedAtUtc = DateTime.UtcNow;
    }

    #endregion

    #region Methods
    public static Result<Conversation> CreatePrivate(string senderId, string otherUserId)
    {
        if (senderId == otherUserId)
            return Result<Conversation>.Failure(ConversationErrors.CannotChatWithSelf);

        var conversation = new Conversation(
            ConversationType.Private,
            title: null,
            createdByUserId: senderId);


        conversation.AddParticipant(senderId, ParticipantRole.Member);
        conversation.AddParticipant(otherUserId, ParticipantRole.Member);

        return Result<Conversation>.Success(conversation);
    }

    public static Result<Conversation> CreateGroup(
        string ownerId,
        string title,
        IEnumerable<string> memberIds)
    {
        var trimmedTitle = title?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(trimmedTitle))
            return Result<Conversation>.Failure(ConversationErrors.TitleRequired);

        var conversation = new Conversation(ConversationType.Group, trimmedTitle, ownerId);
        conversation.AddParticipant(ownerId, ParticipantRole.Owner);

        foreach (var memberId in memberIds.Distinct().Where(id => id != ownerId))
            conversation.AddParticipant(memberId, ParticipantRole.Member);


        return Result<Conversation>.Success(conversation);
    }

    public Result AddParticipant(string userId, ParticipantRole role = ParticipantRole.Member)
    {
        if (ConversationType == ConversationType.Private && Participants.Count >= 2)
            return Result.Failure(ConversationErrors.CannotAddMemberToPrivateConversation);

        if (_participants.Any(p => p.UserId == userId))
            return Result.Failure(ConversationErrors.ParticipantAlreadyExists);

        var result = ConversationParticipant.Create(Id, userId, role);

        if (result.IsSuccess)
            _participants.Add(result.Value!);

        return Result.Success();
    }

    #endregion


    #region Navigation Properties
    public ApplicationUser CreatedBy { get; set; } = null!;

    public IReadOnlyCollection<ConversationParticipant> Participants => _participants.AsReadOnly();


    #endregion
}
