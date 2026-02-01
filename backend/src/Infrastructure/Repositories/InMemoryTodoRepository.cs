using System.Collections.Concurrent;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Repositories;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<string, Todo> _todos = new();

    public Task<string> CreateAsync(Todo todo)
    {
        _todos[todo.Id] = todo;
        return Task.FromResult(todo.Id);
    }

    public Task<List<Todo>> GetByUserAsync(string userId)
    {
        var todos = _todos.Values.Where(t => t.UserId == userId).ToList();
        return Task.FromResult(todos);
    }

    public Task<Todo?> GetByIdAsync(string id, string userId)
    {
        if (_todos.TryGetValue(id, out var todo) && todo.UserId == userId)
        {
            return Task.FromResult<Todo?>(todo);
        }

        return Task.FromResult<Todo?>(null);
    }

    public Task<Todo?> GetByIdAsync(string id)
    {
        _todos.TryGetValue(id, out var todo);
        return Task.FromResult(todo);
    }

    public Task UpdateAsync(Todo todo)
    {
        _todos[todo.Id] = todo;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id, string userId)
    {
        if (_todos.TryGetValue(id, out var todo) && todo.UserId == userId)
        {
            _todos.TryRemove(id, out _);
        }

        return Task.CompletedTask;
    }
}
