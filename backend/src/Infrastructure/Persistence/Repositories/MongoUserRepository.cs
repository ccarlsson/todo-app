using MongoDB.Driver;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Persistence.Models;

namespace TodoApp.Infrastructure.Persistence.Repositories;

public sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<MongoUserDocument> _users;

    public MongoUserRepository(MongoDbContext context)
    {
        _users = context.Users;
    }

    public async Task<string> CreateAsync(User user)
    {
        var document = new MongoUserDocument
        {
            Id = user.Id,
            Email = user.Email.Value,
            PasswordHash = user.PasswordHash
        };

        await _users.InsertOneAsync(document);
        return document.Id;
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        var document = await _users.Find(u => u.Email == email.Value).FirstOrDefaultAsync();
        if (document is null)
        {
            return null;
        }

        var user = new User(Email.Create(document.Email), document.PasswordHash)
        {
            Id = document.Id
        };

        return user;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var document = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        if (document is null)
        {
            return null;
        }

        var user = new User(Email.Create(document.Email), document.PasswordHash)
        {
            Id = document.Id
        };

        return user;
    }
}
