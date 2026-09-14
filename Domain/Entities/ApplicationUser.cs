using Domain.Abstractions;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        #region Constructors
        private ApplicationUser() { }

        private ApplicationUser(
            string displayName,
            string email,
            DateOnly dateOfBirth,
            Gender gender)
        {
            DisplayName = displayName;
            Email = email;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            UserName = email.Split('@')[0];
        }
        #endregion

        #region Properties
        public string DisplayName { get; private set; } = null!;
        public string? ProfilePictureUrl { get; private set; }
        public DateOnly DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        #endregion


        #region Methods
        public static Result<ApplicationUser> Create(
            string displayName,
            string email,
            DateOnly dateOfBirth,
            Gender gender)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                return Result<ApplicationUser>.Failure(new Error("USER.DISPLAY_NAME", "Display name is required", ErrorType.Validation));

            if (string.IsNullOrWhiteSpace(email))
                return Result<ApplicationUser>.Failure(new Error("USER.EMAIL", "Email address is required", ErrorType.Validation));

            return Result<ApplicationUser>.Success(
                new ApplicationUser(
                    displayName.Trim(),
                    email.Trim(),
                    dateOfBirth,
                    gender));
        }

        #endregion

    }
}
