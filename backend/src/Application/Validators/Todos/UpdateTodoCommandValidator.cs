using FluentValidation;
using TodoApp.Application.Commands.Todos;

namespace TodoApp.Application.Validators.Todos;

public sealed class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.TodoId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => x.Title is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
