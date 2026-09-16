using Application.DTO;
using Domain.Abstractions;
using MediatR;

namespace Application.Features.Conversations.CreatePrivate;

public sealed record CreatePrivateCommand(string OtherUserId) : IRequest<Result<ConversationDto>>;