using Domain.Abstractions;
using MediatR;

namespace Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string DisplayName,
    string Password,
    string ConfirmPassword) : IRequest<Result<AuthResponse>>;