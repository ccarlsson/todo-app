using MediatR;
using TodoApp.Application.Interfaces;

namespace TodoApp.Application.Commands.Todos;

public sealed class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, bool>
{
    private readonly ITodoRepository _todos;

    public DeleteTodoCommandHandler(ITodoRepository todos)
    {
        _todos = todos;
    }

    public async Task<bool> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _todos.GetByIdAsync(request.TodoId, request.UserId);
        if (todo is null)
        {
            return false;
        }

        await _todos.DeleteAsync(request.TodoId, request.UserId);
        return true;
    }
}
