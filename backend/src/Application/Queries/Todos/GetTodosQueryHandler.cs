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
            list = [.. list.Where(t => t.Status == request.Status.Value)];
        }

        if (request.Priority.HasValue)
        {
            list = [.. list.Where(t => t.Priority == request.Priority.Value)];
        }

        if (!string.IsNullOrWhiteSpace(request.DueDateFilter))
        {
            var now = DateTime.UtcNow;
            if (request.DueDateFilter.Equals("overdue", StringComparison.OrdinalIgnoreCase))
            {
                list = [.. list.Where(t => t.DueDate.HasValue && t.DueDate.Value < now)];
            }
            else if (request.DueDateFilter.Equals("upcoming", StringComparison.OrdinalIgnoreCase))
            {
                list = [.. list.Where(t => t.DueDate.HasValue && t.DueDate.Value >= now)];
            }
        }

        list = request.SortBy?.ToLowerInvariant() switch
        {
            "duedate" => [.. list.OrderBy(t => t.DueDate ?? DateTime.MaxValue)],
            "createdat" => [.. list.OrderBy(t => t.CreatedAt)],
            _ => [.. list.OrderBy(t => t.CreatedAt)]
        };

        return [.. list.Select(t => new TodoDto(
            t.Id,
            t.Title,
            t.Description,
            t.DueDate,
            t.Priority,
            t.Status,
            t.CreatedAt,
            t.UpdatedAt))];
    }
}
