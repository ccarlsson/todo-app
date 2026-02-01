using MediatR;
using TodoApp.Application.Interfaces;

namespace TodoApp.Application.Commands.Todos;

public sealed class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, bool>
{
    private readonly ITodoRepository _todos;

    public UpdateTodoCommandHandler(ITodoRepository todos)
    {
        _todos = todos;
    }

    public async Task<bool> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todos.GetByIdAsync(request.TodoId, request.UserId);
        if (todo is null)
        {
            return false;
        }

        todo.Update(
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority,
            request.Status);

        await _todos.UpdateAsync(todo);
        return true;
    }
}
