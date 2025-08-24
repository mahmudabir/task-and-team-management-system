using FluentValidation;

namespace Application.UseCases.TaskItems.Update;

public class TaskItemUpdateValidator : AbstractValidator<TaskItemUpdateCommand>
{
    public TaskItemUpdateValidator()
    {
        RuleFor(x => x.Payload).NotNull();
        RuleFor(x => x.Payload.Id).NotEmpty();
        RuleFor(x => x.Payload.Title).NotEmpty();
        RuleFor(x => x.Payload.Description).NotEmpty();
    }
}