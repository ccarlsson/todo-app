using MediatR;
using TodoApp.Api.Models.Auth;
using TodoApp.Application.Commands.Users;

namespace TodoApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (
            RegisterRequest request,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new RegisterUserCommand(request.Email, request.Password));
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "Conflict" => Results.Conflict(result.Error),
                    _ => Results.BadRequest(result.Error)
                };
            }

            var user = result.Value!;
            return Results.Created($"/users/{user.Id}", user);
        });

        app.MapPost("/auth/login", async (
            LoginRequest request,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new LoginUserCommand(request.Email, request.Password));
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "Unauthorized" => Results.Unauthorized(),
                    _ => Results.BadRequest(result.Error)
                };
            }

            var token = result.Value!;
            return Results.Ok(new AuthResponse(token.Token, token.ExpiresIn));
        });

        return app;
    }
}
