using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using server.Application.Interfaces;
using server.Domain.Entities;
using server.Infrastructure.Identity;
using server.Infrastructure.Persistence;
using server.Shared.Abstraction;
using server.Shared.Extensions;
using server.Shared.Helpers;

namespace server.Application.Features.Auth;

public sealed class Login : IEndpoint
{
    public record AuthResponse(
        string Id,
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
        IJwtGenerator jwtGenerator,
        IOptions<JwtSettings> jwtOptions,
        AppDbContext dbContext,
        IHttpContextAccessor contextAccessor) : IRequestHandler<Command, Result<AuthResponse>>
    {
        private readonly JwtSettings jwtSettings = jwtOptions.Value;
        public async Task<Result<AuthResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var httpResponse = contextAccessor.HttpContext?.Response;

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);

            if (!user.EmailConfirmed)
                return Result<AuthResponse>.Failure(UserErrors.EmailNotConfirmed);

            var signInResult = await signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
                return Result<AuthResponse>.Failure(UserErrors.AccountLocked);

            if (!signInResult.Succeeded)
                return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);

            var token = jwtGenerator.Generate(user);
            var refreshToken = RefreshTokenGenerator.GenerateToken();

            var hashed = RefreshTokenHasher.Hash(refreshToken);

            RefreshToken rfToken = new()
            {
                TokenHash = hashed,
                UserId = user.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationInDays)
            };

            dbContext.RefreshTokens.Add(rfToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            CookieHelper.SetRefreshTokenCookie(
                httpResponse!,
                refreshToken,
                rfToken.ExpiresAtUtc);

            var response = new AuthResponse(user.Id, user.DisplayName, user.Email, user.UserName, token);

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
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireRateLimiting("AuthLimiter");
    }

}
