using Application.DTO;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(
        LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<AuthResponseDto>.Failure(UserErrors.NotFound);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
            return Result<AuthResponseDto>.Failure(UserErrors.InvalidCredentials);

        var token = await jwtTokenGenerator.GenerateAccessTokenAsync(user);

        var response = new AuthResponseDto(
          token,
          new UserDto(
              user.Id,
              user.DisplayName,
              user.UserName!,
              user.Email!, user.ProfilePictureUrl));

        return Result<AuthResponseDto>.Success(response);
    }
}
