using Api.Extensions;
using Application.Features.Auth;
using Application.Features.Auth.Register;
using Domain.Abstractions;
using MediatR;

namespace Api.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void MapEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
            RegisterCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            Result<AuthResponse> result = await sender.Send(command, cancellationToken);
            return result.ToApiResponse();
        })
            .WithName("Register")
            .WithTags("Auth");
    }
}
