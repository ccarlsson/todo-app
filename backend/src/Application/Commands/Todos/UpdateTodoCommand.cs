using MediatR;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.Commands.Todos;

public sealed record UpdateTodoCommand(
    string UserId,
    string TodoId,
    string? Title,
    string? Description,
    DateTime? DueDate,
    Priority? Priority,
    TodoStatus? Status) : IRequest<bool>;
