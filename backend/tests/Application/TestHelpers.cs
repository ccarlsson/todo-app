using Microsoft.Extensions.Configuration;
using TodoApp.Application.Interfaces;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Services;

namespace TodoApp.Application.Tests;

public static class TestHelpers
{
    public static IUserRepository CreateUserRepository() => new InMemoryUserRepository();

    public static ITodoRepository CreateTodoRepository() => new InMemoryTodoRepository();

    public static IPasswordHasher CreatePasswordHasher() => new PasswordHasher();

    public static IJwtTokenService CreateJwtTokenService()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "TodoApp",
            ["Jwt:Audience"] = "TodoApp",
            ["Jwt:Key"] = "test-key-please-change-to-32-bytes-min",
            ["Jwt:ExpiresMinutes"] = "60"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new JwtTokenService(configuration);
    }
}
