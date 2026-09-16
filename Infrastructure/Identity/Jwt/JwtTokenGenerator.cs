using Application.Interfaces;
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

    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

        List<Claim> claims = [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        ];

        claims.AddRange(roleClaims);

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);


        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        SecurityToken token = _jwtSecurityTokenHandler.CreateToken(descriptor);

        return _jwtSecurityTokenHandler.WriteToken(token);
    }
}
