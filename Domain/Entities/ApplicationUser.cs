using Domain.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public sealed class ApplicationUser : IdentityUser<string>
{
    #region Constructors
    private ApplicationUser() { }

    private ApplicationUser(
        string displayName,
        string email)
    {
        Id = Guid.NewGuid().ToString();
        DisplayName = displayName;
        Email = email;
        UserName = email.Split('@')[0];
    }
    #endregion

    #region Properties
    private readonly HashSet<Conversation> _conversations = [];
    public string DisplayName { get; private set; } = null!;
    public string? ProfilePictureUrl { get; private set; }
    #endregion

    #region Navigation Props
    public IReadOnlyCollection<Conversation> Conversations => _conversations.AsReadOnly();
    #endregion


    #region Methods
    public static Result<ApplicationUser> Create(
        string displayName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return Result<ApplicationUser>.Failure(
                Error.Validation(
                    "User.DisplayNameRequired",
                    "Display name is required."));

        if (string.IsNullOrWhiteSpace(email))
            return Result<ApplicationUser>.Failure(
                Error.Validation(
                    "User.EmailRequired",
                    "Email address is required."));

        return Result<ApplicationUser>.Success(
            new ApplicationUser(
                displayName.Trim(),
                email.Trim()));
    }

    #endregion

}
