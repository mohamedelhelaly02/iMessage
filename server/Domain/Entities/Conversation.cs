using server.Domain.Enums;

namespace server.Domain.Entities;

public sealed class Conversation : Entity
{
    public string? Title { get; set; }
    public ConversationType Type { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastMessageAtUtc { get; set; }
    public string CreatedById { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public HashSet<Participant> Participants { get; set; } = [];
}
