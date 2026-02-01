using TodoApp.Domain.ValueObjects;

namespace TodoApp.Api.Models.Todos;

public sealed record TodoCreateRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    Priority? Priority);
