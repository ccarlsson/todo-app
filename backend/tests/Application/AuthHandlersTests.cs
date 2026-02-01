using TodoApp.Application.Commands.Users;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using Xunit;

namespace TodoApp.Application.Tests;

public sealed class AuthHandlersTests
{
    [Fact]
    public async Task RegisterUser_CreatesUser_WhenEmailIsUnique()
    {
        IUserRepository users = TestHelpers.CreateUserRepository();
        IPasswordHasher hasher = TestHelpers.CreatePasswordHasher();

        var handler = new RegisterUserCommandHandler(users, hasher);
        var result = await handler.Handle(
            new RegisterUserCommand("user@example.com", "Secret123!"),
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("user@example.com", result.Value?.Email);
    }

    [Fact]
    public async Task RegisterUser_ReturnsConflict_WhenEmailExists()
    {
        IUserRepository users = TestHelpers.CreateUserRepository();
        IPasswordHasher hasher = TestHelpers.CreatePasswordHasher();

        var handler = new RegisterUserCommandHandler(users, hasher);
        await handler.Handle(
            new RegisterUserCommand("user@example.com", "Secret123!"),
            CancellationToken.None);

        var result = await handler.Handle(
            new RegisterUserCommand("user@example.com", "Secret123!"),
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Conflict", result.ErrorCode);
    }

    [Fact]
    public async Task LoginUser_ReturnsToken_WhenCredentialsAreValid()
    {
        IUserRepository users = TestHelpers.CreateUserRepository();
        IPasswordHasher hasher = TestHelpers.CreatePasswordHasher();
        IJwtTokenService jwt = TestHelpers.CreateJwtTokenService();

        var registerHandler = new RegisterUserCommandHandler(users, hasher);
        await registerHandler.Handle(
            new RegisterUserCommand("user@example.com", "Secret123!"),
            CancellationToken.None);

        var loginHandler = new LoginUserCommandHandler(users, hasher, jwt);
        var result = await loginHandler.Handle(
            new LoginUserCommand("user@example.com", "Secret123!"),
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.IsType<AuthTokenDto>(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.Value?.Token));
    }

    [Fact]
    public async Task LoginUser_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        IUserRepository users = TestHelpers.CreateUserRepository();
        IPasswordHasher hasher = TestHelpers.CreatePasswordHasher();
        IJwtTokenService jwt = TestHelpers.CreateJwtTokenService();

        var registerHandler = new RegisterUserCommandHandler(users, hasher);
        await registerHandler.Handle(
            new RegisterUserCommand("user@example.com", "Secret123!"),
            CancellationToken.None);

        var loginHandler = new LoginUserCommandHandler(users, hasher, jwt);
        var result = await loginHandler.Handle(
            new LoginUserCommand("user@example.com", "WrongPassword"),
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Unauthorized", result.ErrorCode);
    }
}
