using Domain.Abstractions.Database.Repositories;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Delete;

[ScopedLifetime]
public class TaskItemDeleteCommandHandler(ITaskItemRepository repository) : CommandHandler<TaskItemDeleteCommand, bool>
{
    public override async Task<Result<bool>> ValidateAsync(TaskItemDeleteCommand command, CqrsContext context, CancellationToken ct = default)
    {
        if (!await repository.ExistsByAsync(x => x.Id == command.Payload, false, ct))
        {
            return Result<bool>.Error()
                               .WithErrorMessage("Validation error.")
                               .AddError(new("Id", ["Not found."]));
        }

        return await base.ValidateAsync(command, context, ct);
    }
    
    public override async Task<Result<bool>> HandleAsync(TaskItemDeleteCommand command, CqrsContext context, CancellationToken ct = default)
    {
        await repository.ExecuteDeleteAsync(x => x.Id == command.Payload, ct);
        await repository.SaveChangesAsync(ct);
        return Result<bool>.Success()
                           .WithPayload(true)
                           .WithMessage("Deleted successfully.");
    }
}