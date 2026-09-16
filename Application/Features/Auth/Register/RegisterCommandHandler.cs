using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Register;

public sealed class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
            return Result<AuthResponse>.Failure(UserErrors.EmailAlreadyExists);

        var userResult = ApplicationUser.Create(request.DisplayName, request.Email);

        if (!userResult.IsSuccess)
            return Result<AuthResponse>.Failure(userResult.Error!);

        var user = userResult.Value;

        var createResult = await userManager.CreateAsync(user!, request.Password);

        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return Result<AuthResponse>.Failure(UserErrors.RegisterationValidation(errors));
        }

        var roleResult = await userManager.AddToRoleAsync(user!, "User");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

            return Result<AuthResponse>.Failure(UserErrors.RoleAssignFailed(errors));
        }

        var token = await jwtTokenGenerator.GenerateAccessTokenAsync(user!);

        return Result<AuthResponse>.Success(new AuthResponse(token));
    }
}
