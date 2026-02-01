using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.DTOs;

public sealed record TodoDto(
    string Id,
    string Title,
    string? Description,
    DateTime? DueDate,
    Priority? Priority,
    TodoStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);
