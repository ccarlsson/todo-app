using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoApp.Infrastructure.Persistence.Models;

namespace TodoApp.Infrastructure.Persistence;

public sealed class MongoDbContext
{
    public IMongoCollection<MongoUserDocument> Users { get; }
    public IMongoCollection<MongoTodoDocument> Todos { get; }

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        Users = database.GetCollection<MongoUserDocument>("users");
        Todos = database.GetCollection<MongoTodoDocument>("todos");

        EnsureIndexes();
    }

    private void EnsureIndexes()
    {
        var userIndex = Builders<MongoUserDocument>.IndexKeys.Ascending(u => u.Email);
        Users.Indexes.CreateOne(new CreateIndexModel<MongoUserDocument>(
            userIndex,
            new CreateIndexOptions { Unique = true }));

        var todoIndex = Builders<MongoTodoDocument>.IndexKeys
            .Ascending(t => t.UserId)
            .Descending(t => t.CreatedAt);
        Todos.Indexes.CreateOne(new CreateIndexModel<MongoTodoDocument>(todoIndex));
    }
}
