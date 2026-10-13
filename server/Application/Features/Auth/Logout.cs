using MediatR;
using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;
using server.Infrastructure.Identity;
using server.Infrastructure.Persistence;
using server.Shared.Abstraction;
using server.Shared.Extensions;
using server.Shared.Helpers;

namespace server.Application.Features.Auth;

public sealed class Logout : IEndpoint
{
    public sealed record Command : IRequest<Result>;

    public sealed class Handler(
        AppDbContext dbContext,
        IHttpContextAccessor contextAccessor) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            string? refreshCookie = CookieHelper.GetRefreshTokenCookie(contextAccessor.HttpContext?.Request!);
            if (!string.IsNullOrEmpty(refreshCookie))
            {
                string tokenHash = RefreshTokenHasher.Hash(refreshCookie);
                RefreshToken? existingToken = await dbContext.RefreshTokens
                    .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);

                if (existingToken is not null && existingToken.RevokedAtUtc is null)
                {
                    await dbContext.RefreshTokens
                        .Where(r => r.FamilyId == existingToken.FamilyId && r.RevokedAtUtc == null)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(r => r.RevokedAtUtc, DateTime.UtcNow)
                            .SetProperty(r => r.IsUsed, true), cancellationToken);
                }
            }

            CookieHelper.DeleteRefreshTokenCookie(contextAccessor.HttpContext?.Response!);

            return Result.Success();
        }
    }


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (ISender sender) =>
        {
            Result logoutResult = await sender.Send(new Command());

            return logoutResult.ToResponse(StatusCodes.Status204NoContent);
        })
            .AllowAnonymous()
            .WithName("Logout")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent);
    }

}
