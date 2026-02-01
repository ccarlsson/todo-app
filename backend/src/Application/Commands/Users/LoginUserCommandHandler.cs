using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.Commands.Users;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, OperationResult<AuthTokenDto>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserCommandHandler(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<OperationResult<AuthTokenDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return OperationResult<AuthTokenDto>.Fail("Email and password are required.", "BadRequest");
        }

        Email email;
        try
        {
            email = Email.Create(request.Email);
        }
        catch (Exception ex)
        {
            return OperationResult<AuthTokenDto>.Fail(ex.Message, "BadRequest");
        }

        var user = await _users.GetByEmailAsync(email);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return OperationResult<AuthTokenDto>.Fail("Invalid credentials.", "Unauthorized");
        }

        var token = _jwtTokenService.CreateToken(user);
        return OperationResult<AuthTokenDto>.Ok(token);
    }
}
