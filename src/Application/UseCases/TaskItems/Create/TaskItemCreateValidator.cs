using FluentValidation;

namespace Application.UseCases.TaskItems.Create;

public class TaskItemCreateValidator : AbstractValidator<TaskItemCreateCommand>
{
    public TaskItemCreateValidator()
    {
        // Attributes in the model are for EF Core and DbContext validation. not for FluentValidation

        // RuleFor methods are for FluentValidation
        RuleFor(x => x.Payload).NotNull();
        RuleFor(x => x.Payload.Id).Empty();
        RuleFor(x => x.Payload.Title).NotEmpty();
        RuleFor(x => x.Payload.Description).NotEmpty();
    }
}