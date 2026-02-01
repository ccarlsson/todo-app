using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Api.Models.Auth;
using TodoApp.Api.Models.Todos;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/register", async (
    RegisterRequest request,
    IUserRepository users,
    IPasswordHasher hasher) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest("Email and password are required.");
    }

    Email email;
    try
    {
        email = Email.Create(request.Email);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

    var existing = await users.GetByEmailAsync(email);
    if (existing is not null)
    {
        return Results.Conflict("Email already exists.");
    }

    var hash = hasher.Hash(request.Password);
    var user = new User(email, hash);
    await users.CreateAsync(user);

    return Results.Created($"/users/{user.Id}", new { user.Id, Email = user.Email.Value });
});

app.MapPost("/auth/login", async (
    LoginRequest request,
    IUserRepository users,
    IPasswordHasher hasher,
    IConfiguration configuration) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest("Email and password are required.");
    }

    Email email;
    try
    {
        email = Email.Create(request.Email);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

    var user = await users.GetByEmailAsync(email);
    if (user is null || !hasher.Verify(request.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    var jwtSection = configuration.GetSection("Jwt");
    var issuer = jwtSection["Issuer"] ?? "TodoApp";
    var audience = jwtSection["Audience"] ?? "TodoApp";
    var key = jwtSection["Key"] ?? "change-this-development-key-please";
    var expiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var exp) ? exp : 60;

    var tokenHandler = new JwtSecurityTokenHandler();
    var keyBytes = Encoding.UTF8.GetBytes(key);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email.Value)
        }),
        Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var jwt = tokenHandler.WriteToken(token);

    return Results.Ok(new AuthResponse(jwt, expiresMinutes * 60));
});

app.MapGet("/todos", async (
    ClaimsPrincipal user,
    ITodoRepository todos,
    string? status,
    string? priority,
    string? sortBy) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var list = await todos.GetByUserAsync(userId);

    if (Enum.TryParse<TodoStatus>(status, true, out var parsedStatus))
    {
        list = list.Where(t => t.Status == parsedStatus).ToList();
    }

    if (Enum.TryParse<Priority>(priority, true, out var parsedPriority))
    {
        list = list.Where(t => t.Priority == parsedPriority).ToList();
    }

    list = sortBy?.ToLowerInvariant() switch
    {
        "duedate" => list.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ToList(),
        "createdat" => list.OrderBy(t => t.CreatedAt).ToList(),
        _ => list.OrderBy(t => t.CreatedAt).ToList()
    };

    var response = list.Select(t => new TodoResponse(
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
    ITodoRepository todos) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var todo = await todos.GetByIdAsync(id, userId);
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
    ITodoRepository todos) =>
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

    var todo = new Todo(
        userId,
        request.Title.Trim(),
        request.Description,
        request.DueDate,
        request.Priority);

    await todos.CreateAsync(todo);

    return Results.Created($"/todos/{todo.Id}", new { todo.Id });
}).RequireAuthorization();

app.MapPut("/todos/{id}", async (
    string id,
    TodoUpdateRequest request,
    ClaimsPrincipal user,
    ITodoRepository todos) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    var todo = await todos.GetByIdAsync(id, userId);
    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.Update(
        request.Title,
        request.Description,
        request.DueDate,
        request.Priority,
        request.Status);

    await todos.UpdateAsync(todo);

    return Results.NoContent();
}).RequireAuthorization();

app.MapDelete("/todos/{id}", async (
    string id,
    ClaimsPrincipal user,
    ITodoRepository todos) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    await todos.DeleteAsync(id, userId);
    return Results.NoContent();
}).RequireAuthorization();

app.Run();
