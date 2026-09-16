using Application.DTO;
using Domain.Abstractions;
using MediatR;

namespace Application.Features.Conversations.GetConversations;

public sealed record GetConversationsQuery : IRequest<Result<List<ConversationDto>>>;
