using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Api.Health;
using TodoApp.Api.Endpoints;
using TodoApp.Application.Behaviors;
using TodoApp.Application.Commands.Users;
using TodoApp.Application.Commands.Todos;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Queries.Todos;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Repositories;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Persistence.Repositories;
using TodoApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks()
    .AddCheck<MongoHealthCheck>("mongodb");
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }
        else
        {
            policy.AllowAnyOrigin();
        }

        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

var mongoSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>();
if (mongoSettings is not null && !string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
{
    builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
    builder.Services.AddSingleton<MongoDbContext>();
    builder.Services.AddSingleton<IUserRepository, MongoUserRepository>();
    builder.Services.AddSingleton<ITodoRepository, MongoTodoRepository>();
}
else
{
    builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
    builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
}

builder.Services.AddMediatR(typeof(CreateTodoCommand).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommand>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"] ?? "TodoApp";
        var audience = jwtSection["Audience"] ?? "TodoApp";
        var key = jwtSection["Key"]
            ?? Environment.GetEnvironmentVariable("JWT_KEY")
            ?? "change-this-development-key-please";

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
else
{
    app.UseHttpsRedirection();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                errors = validationException.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
            return;
        }
    });
});

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapApiEndpoints();

app.Run();
