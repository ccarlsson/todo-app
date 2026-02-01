using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.Commands.Users;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, OperationResult<UserDto>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<OperationResult<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return OperationResult<UserDto>.Fail("Email and password are required.", "BadRequest");
        }

        Email email;
        try
        {
            email = Email.Create(request.Email);
        }
        catch (Exception ex)
        {
            return OperationResult<UserDto>.Fail(ex.Message, "BadRequest");
        }

        var existing = await _users.GetByEmailAsync(email);
        if (existing is not null)
        {
            return OperationResult<UserDto>.Fail("Email already exists.", "Conflict");
        }

        var hash = _passwordHasher.Hash(request.Password);
        var user = new User(email, hash);

        await _users.CreateAsync(user);

        return OperationResult<UserDto>.Ok(new UserDto(user.Id, user.Email.Value));
    }
}
