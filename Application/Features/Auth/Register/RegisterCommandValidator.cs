using FluentValidation;

namespace Application.Features.Auth.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email)
                    .NotEmpty()
                    .WithMessage("{PropertyName} is required.")
                    .EmailAddress()
                    .WithMessage("Invalid email address format.");

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).{8,}$")
            .WithMessage("Password must be at least 8 characters and contain uppercase, lowercase, digit, and special character.");

        RuleFor(c => c.ConfirmPassword)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Equal(c => c.Password)
            .WithMessage("Passwords do not match.");

        RuleFor(c => c.DisplayName)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} cannot exceed 100 characters.");
    }

}
