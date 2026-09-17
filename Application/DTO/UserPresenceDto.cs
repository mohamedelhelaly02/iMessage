namespace Application.DTO;

public sealed class UserPresenceDto(UserDto user, bool isOnline)
{
    public UserDto User { get; private set; } = user;
    public bool IsOnline { get; private set; } = isOnline;
}
