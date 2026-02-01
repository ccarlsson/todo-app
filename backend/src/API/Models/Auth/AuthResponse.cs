namespace TodoApp.Api.Models.Auth;

public sealed record AuthResponse(string Token, int ExpiresIn);
