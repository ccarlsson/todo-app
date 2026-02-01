using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Queries.Todos;

public sealed record GetTodoByIdQuery(string UserId, string TodoId) : IRequest<TodoDto?>;
