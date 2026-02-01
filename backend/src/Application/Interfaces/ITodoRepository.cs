using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces;

public interface ITodoRepository
{
    Task<string> CreateAsync(Todo todo);
    Task<List<Todo>> GetByUserAsync(string userId);
    Task<Todo?> GetByIdAsync(string id, string userId);
    Task UpdateAsync(Todo todo);
    Task DeleteAsync(string id, string userId);
}
