using Domain.Abstractions.Database.Repositories;
using Domain.Entities.TaskItems;

using FluentValidation;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Create;

[ScopedLifetime]
public class TaskItemCreateCommandHandler(IValidator<TaskItemCreateCommand> validator, ITaskItemRepository repository) : CommandHandler<TaskItemCreateCommand, TaskItem>
{
    public override async Task<Result<TaskItem>> ValidateAsync(TaskItemCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<TaskItem>.Error()
                                  .WithErrorMessage("Validation error.")
                                  .WithErrors(validationResult.ToDictionary());
        }

        // if (await repository.ExistsByAsync(x =>
        //                                        x.NameEn == command.Payload.NameEn || x.NameBn == command.Payload.NameBn ||
        //                                        x.NameAr == command.Payload.NameAr || x.NameHi == command.Payload.NameHi,
        //                                    true,
        //                                    ct))
        // {
        //     var errors = TaskItemErrors.AlreadyAdded(command.Payload.NameEn);
        //
        //     return Result<TaskItem>.Error(errors)
        //                           .WithErrorMessage("Validation error.")
        //                           .WithErrors(validationResult.ToDictionary());
        // }

        return Result<TaskItem>.Success();
    }

    public override async Task<Result<TaskItem>> HandleAsync(TaskItemCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        await repository.AddAsync(command.Payload, ct);
        await repository.SaveChangesAsync(ct);
        return Result<TaskItem>.Success()
                              .WithPayload(command.Payload)
                              .WithMessage("Added successfully.");
    }
}