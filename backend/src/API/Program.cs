using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Api.Models.Auth;
using TodoApp.Api.Models.Todos;
using TodoApp.Application.Commands.Users;
using TodoApp.Application.Commands.Todos;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Queries.Todos;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddMediatR(typeof(CreateTodoCommand).Assembly);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"] ?? "TodoApp";
        var audience = jwtSection["Audience"] ?? "TodoApp";
        var key = jwtSection["Key"] ?? "change-this-development-key-please";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

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

app.MapGet("/todos", async (
    ClaimsPrincipal user,
    IMediator mediator,
    string? status,
    string? priority,
    string? sortBy) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var parsedStatus = Enum.TryParse<TodoStatus>(status, true, out var s) ? s : (TodoStatus?)null;
    var parsedPriority = Enum.TryParse<Priority>(priority, true, out var p) ? p : (Priority?)null;

    var todos = await mediator.Send(new GetTodosQuery(userId, parsedStatus, parsedPriority, sortBy));

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
    IMediator mediator) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var todo = await mediator.Send(new GetTodoByIdQuery(userId, id));
    if (todo is null)
    {
        return Results.NotFound();
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
    IMediator mediator) =>
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

    return updated ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/todos/{id}", async (
    string id,
    ClaimsPrincipal user,
    IMediator mediator) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var deleted = await mediator.Send(new DeleteTodoCommand(userId, id));
    return deleted ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.Run();
