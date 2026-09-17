namespace Application.DTO;

public sealed record UserDto(
    string Id,
    string DisplayName,
    string? UserName,
    string? Email,
    string? ProfilePictureUrl,
    DateTime? LastSeenAtUtc);

public sealed record AuthResponseDto(string Token, UserDto User);