using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
}
