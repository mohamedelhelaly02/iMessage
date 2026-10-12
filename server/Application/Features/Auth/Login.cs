using System;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using server.Application.Interfaces;
using server.Domain.Entities;
using server.Shared.Abstraction;
using server.Shared.Extensions;
using server.Shared.Helpers;

namespace server.Application.Features.Auth;

public class Login : IEndpoint
{

    public record AuthResponse(
        string DisplayName,
        string? Email,
        string? UserName,
        string Token);

    public sealed record Command(string Email, string Password) : IRequest<Result<AuthResponse>>;


    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }

    public sealed class Handler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtGenerator jwtGenerator) : IRequestHandler<Command, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result<AuthResponse>.Failure(UserErrors.InvalidCredentails);

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
                return Result<AuthResponse>.Failure(UserErrors.InvalidCredentails);

            var token = jwtGenerator.Generate(user);
            var response = new AuthResponse(user.DisplayName, user.Email, user.UserName, token);

            return Result<AuthResponse>.Success(response);
        }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (Command command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToResponse();
        })
        .WithName("Login")
        .WithTags("Auth")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }

}
