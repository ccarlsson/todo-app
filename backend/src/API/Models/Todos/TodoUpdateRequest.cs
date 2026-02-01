using TodoApp.Domain.ValueObjects;

namespace TodoApp.Api.Models.Todos;

public sealed record TodoUpdateRequest(
    string? Title,
    string? Description,
    DateTime? DueDate,
    Priority? Priority,
    TodoStatus? Status);
