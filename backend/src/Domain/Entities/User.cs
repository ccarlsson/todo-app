using TodoApp.Domain.ValueObjects;

namespace TodoApp.Domain.Entities;

public sealed class User
{
    public string Id { get; init; }
    public Email Email { get; init; }
    public string PasswordHash { get; private set; }

    public User(Email email, string passwordHash)
    {
        Id = Guid.NewGuid().ToString("N");
        Email = email;
        PasswordHash = passwordHash;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
}
