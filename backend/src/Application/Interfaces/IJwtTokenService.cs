using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces;

public interface IJwtTokenService
{
    AuthTokenDto CreateToken(User user);
}
