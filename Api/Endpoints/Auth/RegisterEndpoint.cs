using Api.Extensions;
using Application.Features.Auth;
using Application.Features.Auth.Register;
using Domain.Abstractions;
using MediatR;

namespace Api.Endpoints.Auth;

public static class RegisterEndpoint
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapEndpoint()
        {
            app.MapPost("/api/auth/register",
                async (
                    RegisterCommand command,
                    ISender sender,
                    CancellationToken cancellationToken) =>
            {
                Result<AuthResponse> result = await sender.Send(command, cancellationToken);
                return result.ToApiResponse();
            })
               .WithName("Register")
               .WithTags("Auth")
               .Produces<AuthResponse>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .ProducesProblem(StatusCodes.Status409Conflict)
               .ProducesProblem(StatusCodes.Status500InternalServerError)
               .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
               .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }
}