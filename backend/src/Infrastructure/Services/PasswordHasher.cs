using Microsoft.AspNetCore.Identity;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();
    private static readonly User PlaceholderUser =
        new(Domain.ValueObjects.Email.Create("placeholder@example.com"), string.Empty);

    public string Hash(string password)
    {
        return _hasher.HashPassword(PlaceholderUser, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(
            PlaceholderUser,
            passwordHash,
            password);

        return result == PasswordVerificationResult.Success;
    }
}
