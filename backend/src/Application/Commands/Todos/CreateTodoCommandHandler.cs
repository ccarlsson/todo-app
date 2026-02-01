using MediatR;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Commands.Todos;

public sealed class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, string>
{
    private readonly ITodoRepository _todos;

    public CreateTodoCommandHandler(ITodoRepository todos)
    {
        _todos = todos;
    }

    public async Task<string> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new Todo(
            request.UserId,
            request.Title.Trim(),
            request.Description,
            request.DueDate,
            request.Priority);

        return await _todos.CreateAsync(todo);
    }
}
