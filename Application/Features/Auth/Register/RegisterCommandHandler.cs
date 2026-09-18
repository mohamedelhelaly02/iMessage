using Application.DTO;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Register;

public sealed class RegisterCommandHandler(
    ILogger<RegisterCommandHandler> logger,
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(
        RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
            return Result<AuthResponseDto>.Failure(UserErrors.EmailAlreadyExists);

        var userResult = ApplicationUser.Create(request.DisplayName, request.Email);

        if (!userResult.IsSuccess)
            return Result<AuthResponseDto>.Failure(userResult.Error!);

        ApplicationUser user = userResult.Value!;

        var createResult = await userManager.CreateAsync(user!, request.Password);

        if (!createResult.Succeeded)
        {

            var errors = createResult.Errors
                .GroupBy(e => char.ToLowerInvariant(e.Code[0]) + e.Code[1..])
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(x => x.Description).ToArray()
                );


            return Result<AuthResponseDto>.Failure(UserErrors.RegisterationValidation(errors));
        }

        var roleResult = await userManager.AddToRoleAsync(user!, "User");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

            return Result<AuthResponseDto>.Failure(UserErrors.RoleAssignFailed(errors));
        }

        var token = await jwtTokenGenerator.GenerateAccessTokenAsync(user!);

        var response = new AuthResponseDto(
            token,
            new UserDto(
                user.Id,
                user.DisplayName,
                user.UserName,
                user.Email,
                user.ProfilePictureUrl,
                user.LastSeenAtUtc));

        return Result<AuthResponseDto>.Success(response);
    }
}
