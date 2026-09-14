using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string DisplayName,
    DateOnly DateOfBirth,
    Gender Gender,
    string Password,
    string ConfirmPassword) : IRequest<Result<AuthResponse>>;