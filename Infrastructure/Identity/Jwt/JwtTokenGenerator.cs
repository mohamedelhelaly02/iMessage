using Application.Abstractions;
using Application.Utility;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity.Jwt;

internal sealed class JwtTokenGenerator(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var userClaims = await userManager.GetClaimsAsync(user);
        var userRoles = await userManager.GetRolesAsync(user);

        var roleClaims = userRoles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.DisplayName),
            new(CustomClaims.DateOfBirth, user.DateOfBirth.ToString("yyyy-MM-dd")),
            new(CustomClaims.Gender, user.Gender.ToString())
        };

        claims.AddRange(roleClaims);
        claims.AddRange(userClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMins),
            signingCredentials: signingCredentials);

        return _jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
    }
}
