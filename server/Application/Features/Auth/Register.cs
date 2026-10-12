using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using server.Domain.Entities;
using server.Shared.Abstraction;
using server.Shared.Extensions;
using server.Shared.Helpers;

namespace server.Application.Features.Auth;

public class Register : IEndpoint
{
    public sealed record Command(
        string DisplayName,
        string Email,
        string Password) : IRequest<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage("Display name is required.")
                .MaximumLength(100).WithMessage("Display name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).{8,}$")
                .WithMessage("Password must be at least 8 characters and contain uppercase, lowercase, digit, and special character.");
        }
    }


    public sealed class Handler(
        UserManager<User> userManager) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
                return Result.Failure(UserErrors.EmailInUse);

            User user = new()
            {
                DisplayName = request.DisplayName,
                Email = request.Email,
                UserName = request.Email.Split('@')[0]
            };

            var createResult = await userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                var errorDictionary = new Dictionary<string, List<string>>();


                foreach (var error in createResult.Errors)
                {
                    string fieldName = error.Code.Contains("Password", StringComparison.OrdinalIgnoreCase)
                        ? "password"
                        : "general";

                    if (!errorDictionary.ContainsKey(fieldName))
                        errorDictionary[fieldName] = [];

                    errorDictionary[fieldName].Add(error.Description);

                }

                var formattedErrors = errorDictionary.ToDictionary(k => k.Key, v => v.Value.ToArray());

                return Result.Failure(UserErrors.RegistrationFailed(formattedErrors));
            }

            var addToRoleResult = await userManager.AddToRoleAsync(user, "User");

            if (!addToRoleResult.Succeeded)
            {
                var errors = addToRoleResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                return Result.Failure(UserErrors.RoleAssignmentFailed(errors));
            }

            return Result.Success();
        }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (Command command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToResponse(StatusCodes.Status204NoContent);
        })
        .AllowAnonymous()
        .WithName("Register")
        .WithTags("Auth")
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status204NoContent);
    }

}
