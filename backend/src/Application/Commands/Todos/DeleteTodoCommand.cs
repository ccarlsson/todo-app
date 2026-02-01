using MediatR;

namespace TodoApp.Application.Commands.Todos;

public sealed record DeleteTodoCommand(string UserId, string TodoId) : IRequest<bool>;
