using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;

namespace TodoApp.Application.Queries.Todos;

public sealed class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoDto?>
{
    private readonly ITodoRepository _todos;

    public GetTodoByIdQueryHandler(ITodoRepository todos)
    {
        _todos = todos;
    }

    public async Task<TodoDto?> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _todos.GetByIdAsync(request.TodoId, request.UserId);
        if (todo is null)
        {
            return null;
        }

        return new TodoDto(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.DueDate,
            todo.Priority,
            todo.Status,
            todo.CreatedAt,
            todo.UpdatedAt);
    }
}
