using Application.Abstractions;
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
        var userRoles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        claims.AddRange(
            userRoles.Select(role =>
                new Claim(ClaimTypes.Role, role))
        );

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            signingCredentials: credentials
        );

        return _jwtSecurityTokenHandler.WriteToken(token);
    }
}
