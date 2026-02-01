using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Commands.Users;

public sealed record RegisterUserCommand(string Email, string Password)
    : IRequest<OperationResult<UserDto>>;
