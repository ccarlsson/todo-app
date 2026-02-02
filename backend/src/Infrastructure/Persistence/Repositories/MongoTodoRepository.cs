using MongoDB.Driver;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Models;

namespace TodoApp.Infrastructure.Persistence.Repositories;

public sealed class MongoTodoRepository : ITodoRepository
{
    private readonly IMongoCollection<MongoTodoDocument> _todos;

    public MongoTodoRepository(MongoDbContext context)
    {
        _todos = context.Todos;
    }

    public async Task<string> CreateAsync(Todo todo)
    {
        var document = ToDocument(todo);
        await _todos.InsertOneAsync(document);
        return document.Id;
    }

    public async Task<List<Todo>> GetByUserAsync(string userId)
    {
        var documents = await _todos.Find(t => t.UserId == userId).ToListAsync();
        return [.. documents.Select(ToDomain)];
    }

    public async Task<Todo?> GetByIdAsync(string id, string userId)
    {
        var document = await _todos.Find(t => t.Id == id && t.UserId == userId).FirstOrDefaultAsync();
        return document is null ? null : ToDomain(document);
    }

    public async Task<Todo?> GetByIdAsync(string id)
    {
        var document = await _todos.Find(t => t.Id == id).FirstOrDefaultAsync();
        return document is null ? null : ToDomain(document);
    }

    public async Task UpdateAsync(Todo todo)
    {
        var document = ToDocument(todo);
        await _todos.ReplaceOneAsync(t => t.Id == todo.Id && t.UserId == todo.UserId, document);
    }

    public async Task DeleteAsync(string id, string userId)
    {
        await _todos.DeleteOneAsync(t => t.Id == id && t.UserId == userId);
    }

    private static MongoTodoDocument ToDocument(Todo todo) => new()
    {
        Id = todo.Id,
        UserId = todo.UserId,
        Title = todo.Title,
        Description = todo.Description,
        DueDate = todo.DueDate,
        Priority = todo.Priority,
        Status = todo.Status,
        CreatedAt = todo.CreatedAt,
        UpdatedAt = todo.UpdatedAt
    };

    private static Todo ToDomain(MongoTodoDocument document) => new(
        document.Id,
        document.UserId,
        document.Title,
        document.Description,
        document.DueDate,
        document.Priority,
        document.Status,
        document.CreatedAt,
        document.UpdatedAt);
}
