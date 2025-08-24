using Domain.Abstractions.Database.Repositories;
using Domain.Entities.TaskItems;

using FluentValidation;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Update;

[ScopedLifetime]
public class TaskItemUpdateCommandHandler(IValidator<TaskItemUpdateCommand> validator, ITaskItemRepository repository) : CommandHandler<TaskItemUpdateCommand, TaskItem>
{
    public override async Task<Result<TaskItem>> ValidateAsync(TaskItemUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {

        if (!await repository.ExistsByAsync(x => x.Id == command.Id, false, ct))
        {
            return Result<TaskItem>.Error()
                                  .WithErrorMessage("Validation error.")
                                  .AddError(new("Id", ["Not found."]));
        }

        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<TaskItem>.Error()
                                  .WithErrorMessage("Validation error.")
                                  .WithErrors(validationResult.ToDictionary());
        }

        // if (await repository.ExistsByAsync(x =>
        //                                        x.Id != command.Id &&
        //                                        (x.NameEn == command.Payload.NameEn || x.NameBn == command.Payload.NameBn ||
        //                                         x.NameAr == command.Payload.NameAr || x.NameHi == command.Payload.NameHi),
        //                                    true,
        //                                    ct))
        // {
        //     var errors = TaskItemErrors.AlreadyAdded(command.Payload.NameEn);
        //
        //     return Result<TaskItem>.Error(errors)
        //                           .WithErrorMessage("Validation error.")
        //                           .WithErrors(validationResult.ToDictionary());
        // }

        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<TaskItem>> HandleAsync(TaskItemUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        await repository.ExecuteUpdateAsync(x => x.Id == command.Id, command.Payload, ct);
        await repository.SaveChangesAsync(ct);

        return Result<TaskItem>.Success()
                              .WithPayload(command.Payload)
                              .WithMessage("Updated successfully.");
    }
}