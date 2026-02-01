using System.Collections.Concurrent;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Infrastructure.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    public Task<string> CreateAsync(User user)
    {
        _users[user.Id] = user;
        return Task.FromResult(user.Id);
    }

    public Task<User?> GetByEmailAsync(Email email)
    {
        var user = _users.Values.FirstOrDefault(u => u.Email.Value == email.Value);
        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(string id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }
}
