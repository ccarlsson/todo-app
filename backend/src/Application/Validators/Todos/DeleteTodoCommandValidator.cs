using FluentValidation;
using TodoApp.Application.Commands.Todos;

namespace TodoApp.Application.Validators.Todos;

public sealed class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.TodoId)
            .NotEmpty();
    }
}
