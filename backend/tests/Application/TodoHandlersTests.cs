using TodoApp.Application.Commands.Todos;
using TodoApp.Application.Interfaces;
using TodoApp.Application.Queries.Todos;
using TodoApp.Domain.ValueObjects;
using Xunit;

namespace TodoApp.Application.Tests;

public sealed class TodoHandlersTests
{
    [Fact]
    public async Task CreateTodo_ReturnsId_AndIsRetrievable()
    {
        ITodoRepository todos = TestHelpers.CreateTodoRepository();

        var createHandler = new CreateTodoCommandHandler(todos);
        var todoId = await createHandler.Handle(
            new CreateTodoCommand("user-1", "Buy milk", "lactose free", null, Priority.High),
            CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(todoId));

        var getHandler = new GetTodoByIdQueryHandler(todos);
        var todo = await getHandler.Handle(
            new GetTodoByIdQuery("user-1", todoId),
            CancellationToken.None);

        Assert.NotNull(todo);
        Assert.Equal("Buy milk", todo?.Title);
        Assert.Equal(Priority.High, todo?.Priority);
    }

    [Fact]
    public async Task UpdateTodo_ChangesStatus()
    {
        ITodoRepository todos = TestHelpers.CreateTodoRepository();

        var createHandler = new CreateTodoCommandHandler(todos);
        var todoId = await createHandler.Handle(
            new CreateTodoCommand("user-1", "Write tests", null, null, Priority.Medium),
            CancellationToken.None);

        var updateHandler = new UpdateTodoCommandHandler(todos);
        var updated = await updateHandler.Handle(
            new UpdateTodoCommand(
                "user-1",
                todoId,
                null,
                null,
                null,
                null,
                TodoStatus.Completed),
            CancellationToken.None);

        Assert.True(updated);

        var getHandler = new GetTodoByIdQueryHandler(todos);
        var todo = await getHandler.Handle(
            new GetTodoByIdQuery("user-1", todoId),
            CancellationToken.None);

        Assert.Equal(TodoStatus.Completed, todo?.Status);
    }

    [Fact]
    public async Task DeleteTodo_RemovesItem()
    {
        ITodoRepository todos = TestHelpers.CreateTodoRepository();

        var createHandler = new CreateTodoCommandHandler(todos);
        var todoId = await createHandler.Handle(
            new CreateTodoCommand("user-1", "Remove me", null, null, Priority.Low),
            CancellationToken.None);

        var deleteHandler = new DeleteTodoCommandHandler(todos);
        var deleted = await deleteHandler.Handle(
            new DeleteTodoCommand("user-1", todoId),
            CancellationToken.None);

        Assert.True(deleted);

        var getHandler = new GetTodoByIdQueryHandler(todos);
        var todo = await getHandler.Handle(
            new GetTodoByIdQuery("user-1", todoId),
            CancellationToken.None);

        Assert.Null(todo);
    }

    [Fact]
    public async Task GetTodos_FiltersByStatusAndPriority()
    {
        ITodoRepository todos = TestHelpers.CreateTodoRepository();

        var createHandler = new CreateTodoCommandHandler(todos);
        var todoId = await createHandler.Handle(
            new CreateTodoCommand("user-1", "First", null, null, Priority.High),
            CancellationToken.None);

        var updateHandler = new UpdateTodoCommandHandler(todos);
        await updateHandler.Handle(
            new UpdateTodoCommand(
                "user-1",
                todoId,
                null,
                null,
                null,
                null,
                TodoStatus.InProgress),
            CancellationToken.None);

        await createHandler.Handle(
            new CreateTodoCommand("user-1", "Second", null, null, Priority.Low),
            CancellationToken.None);

        var listHandler = new GetTodosQueryHandler(todos);
        var filtered = await listHandler.Handle(
            new GetTodosQuery("user-1", TodoStatus.InProgress, Priority.High, "createdAt"),
            CancellationToken.None);

        Assert.Single(filtered);
        Assert.Equal("First", filtered[0].Title);
    }
}
