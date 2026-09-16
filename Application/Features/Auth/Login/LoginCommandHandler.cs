using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<AuthResponse>.Failure(UserErrors.NotFound);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
            return Result<AuthResponse>.Failure(UserErrors.InvalidCredentials);

        var token = await jwtTokenGenerator.GenerateAccessTokenAsync(user);

        return Result<AuthResponse>.Success(new AuthResponse(token));
    }
}
