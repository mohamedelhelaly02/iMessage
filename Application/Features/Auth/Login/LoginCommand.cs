using Application.DTO;
using Domain.Abstractions;
using MediatR;

namespace Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponseDto>>;
