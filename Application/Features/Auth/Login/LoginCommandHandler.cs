using Application.Abstractions;
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
            return Result<AuthResponse>.Failure(new Error("USER.CONFLICT", "Invalid Email Or Password", ErrorType.Conflict));

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
            return Result<AuthResponse>.Failure(new Error("USER.CONFLICT", "Invalid Email Or Password", ErrorType.Conflict));

        var token = await jwtTokenGenerator.GenerateJwtTokenAsync(user);

        return Result<AuthResponse>.Success(new AuthResponse(token));
    }
}
