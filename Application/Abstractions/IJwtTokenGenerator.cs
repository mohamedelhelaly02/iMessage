using Domain.Entities;

namespace Application.Abstractions;

public interface IJwtTokenGenerator
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);
}
