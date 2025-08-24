using Application.Mappers.TaskItems;

using Domain.Abstractions.Database.Repositories;
using Domain.Entities.TaskItems;

using FluentValidation;

using Shared.Models.TaskItems;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Update;

[ScopedLifetime]
public class TaskItemUpdateCommandHandler(IValidator<TaskItemUpdateCommand> validator, ITaskItemRepository repository) : CommandHandler<TaskItemUpdateCommand, TaskItemViewModel>
{
    public override async Task<Result<TaskItemViewModel>> ValidateAsync(TaskItemUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {

        if (!await repository.ExistsByAsync(x => x.Id == command.Id, false, ct))
        {
            return Result<TaskItemViewModel>.Error()
                                            .WithErrorMessage("Validation error.")
                                            .AddError(new("Id", ["Not found."]));
        }

        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<TaskItemViewModel>.Error()
                                            .WithErrorMessage("Validation error.")
                                            .WithErrors(validationResult.ToDictionary());
        }
        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<TaskItemViewModel>> HandleAsync(TaskItemUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var task = command.Payload.ToTaskItem();
        await repository.ExecuteUpdateAsync(x => x.Id == command.Id, task, ct);
        await repository.SaveChangesAsync(ct);

        return Result<TaskItemViewModel>.Success()
                                        .WithPayload(task.ToTaskItemViewModel())
                                        .WithMessage("Updated successfully.");
    }
}