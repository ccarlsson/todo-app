using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;

namespace TodoApp.Application.Queries.Todos;

public sealed class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, List<TodoDto>>
{
    private readonly ITodoRepository _todos;

    public GetTodosQueryHandler(ITodoRepository todos)
    {
        _todos = todos;
    }

    public async Task<List<TodoDto>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var list = await _todos.GetByUserAsync(request.UserId);

        if (request.Status.HasValue)
        {
            list = list.Where(t => t.Status == request.Status.Value).ToList();
        }

        if (request.Priority.HasValue)
        {
            list = list.Where(t => t.Priority == request.Priority.Value).ToList();
        }

        list = request.SortBy?.ToLowerInvariant() switch
        {
            "duedate" => list.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList(),
            "createdat" => list.OrderBy(t => t.CreatedAt).ToList(),
            _ => list.OrderBy(t => t.CreatedAt).ToList()
        };

        return list.Select(t => new TodoDto(
            t.Id,
            t.Title,
            t.Description,
            t.DueDate,
            t.Priority,
            t.Status,
            t.CreatedAt,
            t.UpdatedAt)).ToList();
    }
}
