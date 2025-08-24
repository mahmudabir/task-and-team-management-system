using Application.Mappers.TaskItems;

using Domain.Abstractions.Database.Repositories;
using Domain.Entities.TaskItems;

using FluentValidation;

using Shared.Models.TaskItems;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Create;

[ScopedLifetime]
public class TaskItemCreateCommandHandler(IValidator<TaskItemCreateCommand> validator, ITaskItemRepository repository) : CommandHandler<TaskItemCreateCommand, TaskItemViewModel>
{
    public override async Task<Result<TaskItemViewModel>> ValidateAsync(TaskItemCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<TaskItemViewModel>.Error()
                                            .WithErrorMessage("Validation error.")
                                            .WithErrors(validationResult.ToDictionary());
        }

        return Result<TaskItemViewModel>.Success();
    }

    public override async Task<Result<TaskItemViewModel>> HandleAsync(TaskItemCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var task = command.Payload.ToTaskItem();
        await repository.AddAsync(task, ct);
        await repository.SaveChangesAsync(ct);
        return Result<TaskItemViewModel>.Success()
                                        .WithPayload(task.ToTaskItemViewModel())
                                        .WithMessage("Added successfully.");
    }
}