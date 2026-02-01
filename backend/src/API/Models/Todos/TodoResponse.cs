using TodoApp.Domain.ValueObjects;

namespace TodoApp.Api.Models.Todos;

public sealed record TodoResponse(
    string Id,
    string Title,
    string? Description,
    DateTime? DueDate,
    Priority? Priority,
    TodoStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);
