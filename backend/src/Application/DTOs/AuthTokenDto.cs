namespace TodoApp.Application.DTOs;

public sealed record AuthTokenDto(string Token, int ExpiresIn);
