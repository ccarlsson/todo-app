using FluentValidation;
using TodoApp.Application.Commands.Todos;

namespace TodoApp.Application.Validators.Todos;

public sealed class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
