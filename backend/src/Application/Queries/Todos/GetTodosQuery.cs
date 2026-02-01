using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.Queries.Todos;

public sealed record GetTodosQuery(
    string UserId,
    TodoStatus? Status,
    Priority? Priority,
    string? SortBy) : IRequest<List<TodoDto>>;
