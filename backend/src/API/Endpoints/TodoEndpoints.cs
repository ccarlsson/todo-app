using System.Security.Claims;
using MediatR;
using TodoApp.Api.Models.Todos;
using TodoApp.Application.Commands.Todos;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Queries.Todos;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Api.Endpoints;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/todos", async (
            ClaimsPrincipal user,
            IMediator mediator,
            string? status,
            string? priority,
            string? sortBy,
            string? dueDate) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var parsedStatus = Enum.TryParse<TodoStatus>(status, true, out var s) ? s : (TodoStatus?)null;
            var parsedPriority = Enum.TryParse<Priority>(priority, true, out var p) ? p : (Priority?)null;

            var todos = await mediator.Send(new GetTodosQuery(userId, parsedStatus, parsedPriority, sortBy, dueDate));

            var response = todos.Select(t => new TodoResponse(
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                t.Priority,
                t.Status,
                t.CreatedAt,
                t.UpdatedAt));

            return Results.Ok(response);
        }).RequireAuthorization();

        app.MapGet("/todos/{id}", async (
            string id,
            ClaimsPrincipal user,
            IMediator mediator,
            ITodoRepository todosRepository) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var todo = await mediator.Send(new GetTodoByIdQuery(userId, id));
            if (todo is null)
            {
                var existing = await todosRepository.GetByIdAsync(id);
                return existing is null ? Results.NotFound() : Results.Forbid();
            }

            var response = new TodoResponse(
                todo.Id,
                todo.Title,
                todo.Description,
                todo.DueDate,
                todo.Priority,
                todo.Status,
                todo.CreatedAt,
                todo.UpdatedAt);

            return Results.Ok(response);
        }).RequireAuthorization();

        app.MapPost("/todos", async (
            TodoCreateRequest request,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest("Title is required.");
            }

            var todoId = await mediator.Send(new CreateTodoCommand(
                userId,
                request.Title,
                request.Description,
                request.DueDate,
                request.Priority));

            return Results.Created($"/todos/{todoId}", new { Id = todoId });
        }).RequireAuthorization();

        app.MapPut("/todos/{id}", async (
            string id,
            TodoUpdateRequest request,
            ClaimsPrincipal user,
            IMediator mediator,
            ITodoRepository todosRepository) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var updated = await mediator.Send(new UpdateTodoCommand(
                userId,
                id,
                request.Title,
                request.Description,
                request.DueDate,
                request.Priority,
                request.Status));

            if (updated)
            {
                return Results.NoContent();
            }

            var existing = await todosRepository.GetByIdAsync(id);
            return existing is null ? Results.NotFound() : Results.Forbid();
        }).RequireAuthorization();

        app.MapDelete("/todos/{id}", async (
            string id,
            ClaimsPrincipal user,
            IMediator mediator,
            ITodoRepository todosRepository) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Results.Unauthorized();
            }

            var deleted = await mediator.Send(new DeleteTodoCommand(userId, id));
            if (deleted)
            {
                return Results.NoContent();
            }

            var existing = await todosRepository.GetByIdAsync(id);
            return existing is null ? Results.NotFound() : Results.Forbid();
        }).RequireAuthorization();

        return app;
    }
}
