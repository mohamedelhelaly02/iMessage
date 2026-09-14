using Application.Abstractions;
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
            return Result<AuthResponse>.Failure(new Error(
                "USER.CONFLICT", "There exists a user with same email address", ErrorType.Conflict));

        var userResult = ApplicationUser.Create(request.DisplayName, request.Email, request.DateOfBirth, request.Gender);

        if (!userResult.IsSuccess)
            return Result<AuthResponse>.Failure(userResult.Error!);

        var user = userResult.Value;

        var result = await userManager.CreateAsync(user!, request.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result<AuthResponse>.Failure(
                new Error(
                "USER.VALIDATION",
                $"Validation errors occured: {error?.Description ?? ""}", ErrorType.Validation));
        }

        await userManager.AddToRoleAsync(user!, "User");

        var token = await jwtTokenGenerator.GenerateJwtTokenAsync(user!);

        return Result<AuthResponse>.Success(new AuthResponse(token));
    }
}
