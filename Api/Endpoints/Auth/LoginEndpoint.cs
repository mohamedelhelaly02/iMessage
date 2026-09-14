using Api.Extensions;
using Application.Features.Auth;
using Application.Features.Auth.Login;
using Domain.Abstractions;
using MediatR;

namespace Api.Endpoints.Auth;

public static class LoginEndpoint
{
    extension(IEndpointRouteBuilder app)
    {
        public void MapEndpoint()
        {
            app.MapPost("/api/auth/login",
                async (
                    LoginCommand command,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    Result<AuthResponse> result = await sender.Send(command, cancellationToken);
                    return result.ToApiResponse();
                })
               .WithName("Login")
               .WithTags("Auth")
               .Produces<AuthResponse>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .ProducesProblem(StatusCodes.Status500InternalServerError)
               .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
               .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }
}
