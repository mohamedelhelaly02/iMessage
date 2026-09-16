namespace Application.DTO;

public sealed record UserDto(
    string Id,
    string DisplayName,
    string UserName,
    string Email,
    string? ProfilePictureUrl);

public sealed record AuthResponseDto(string Token, UserDto User);