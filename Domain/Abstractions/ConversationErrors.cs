namespace Domain.Abstractions;

public static class ConversationErrors
{
    public static Error CannotChatWithSelf =>
        Error.Validation(
            "Conversation.CannotChatWithSelf",
            "You cannot create a private conversation with yourself.");

    public static Error TitleRequired =>
        Error.Validation(
            "Conversation.TitleRequired",
            "Conversation title is required.");

    public static Error CannotAddMemberToPrivateConversation =>
        Error.Validation(
            "Conversation.CannotAddMemberToPrivateConversation",
            "Cannot add members to a private conversation.");

    public static Error ParticipantAlreadyExists =>
        Error.Conflict(
            "Conversation.ParticipantAlreadyExists",
            "The user is already a participant in this conversation.");
}
