using Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public sealed class ApplicationUser : IdentityUser<string>
{
    private readonly HashSet<Conversation> _conversations = [];

    private ApplicationUser() { }

    private ApplicationUser(string displayName, string email)
    {
        Id = Guid.NewGuid().ToString();
        DisplayName = displayName;
        Email = email;
        UserName = email;
    }

    public string DisplayName { get; private set; } = null!;
    public string? ProfilePictureUrl { get; private set; }
    public DateTime? LastSeenAtUtc { get; private set; }

    public IReadOnlyCollection<Conversation> Conversations =>
        _conversations.AsReadOnly();

    public static Result<ApplicationUser> Create(string displayName, string email)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result<ApplicationUser>.Failure(
                Error.Validation(
                    "User.DisplayNameRequired",
                    "Display name is required."));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<ApplicationUser>.Failure(
                Error.Validation(
                    "User.EmailRequired",
                    "Email address is required."));
        }

        email = email.Trim();

        return Result<ApplicationUser>.Success(
            new ApplicationUser(displayName.Trim(), email));
    }

    public Result UpdateProfile(string displayName, string? profilePictureUrl)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result.Failure(
                Error.Validation(
                    "User.DisplayNameRequired",
                    "Display name is required."));
        }

        DisplayName = displayName.Trim();
        ProfilePictureUrl = string.IsNullOrWhiteSpace(profilePictureUrl)
            ? null
            : profilePictureUrl.Trim();

        return Result.Success();
    }

    public void MarkAsSeen() => LastSeenAtUtc = DateTime.UtcNow;
}