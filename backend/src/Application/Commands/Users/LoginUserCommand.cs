using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Commands.Users;

public sealed record LoginUserCommand(string Email, string Password)
    : IRequest<OperationResult<AuthTokenDto>>;
