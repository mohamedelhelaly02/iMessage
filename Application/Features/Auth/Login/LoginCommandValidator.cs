using FluentValidation;

namespace Application.Features.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
                   .NotEmpty()
                   .WithMessage("{PropertyName} is required.")
                   .EmailAddress()
                   .WithMessage("Invalid email address format.");

        RuleFor(c => c.Password)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");
    }
}
