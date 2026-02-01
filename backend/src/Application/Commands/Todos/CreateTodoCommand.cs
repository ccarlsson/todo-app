using MediatR;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.Commands.Todos;

public sealed record CreateTodoCommand(
    string UserId,
    string Title,
    string? Description,
    DateTime? DueDate,
    Priority? Priority) : IRequest<string>;
