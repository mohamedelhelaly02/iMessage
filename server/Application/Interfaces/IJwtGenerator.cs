using server.Domain.Entities;

namespace server.Application.Interfaces;

public interface IJwtGenerator
{
    string Generate(User user);
}
