using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Update;

[ScopedLifetime]
public class TaskItemUpdateStatusCommandHandler(ITaskItemRepository repository, UserManager<ApplicationUser> userManager, IHttpContextService httpContextService) : CommandHandler<TaskItemUpdateStatusCommand, bool>
{
    public override async Task<Result<bool>> ValidateAsync(TaskItemUpdateStatusCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var username = httpContextService.GetCurrentUserIdentity();
        var user = await userManager.FindByNameAsync(username);
        if (!await repository.ExistsByAsync(x => x.Id == command.Id && x.AssignedToUserId == user.Id, false, ct))
        {
            return Result<bool>.Error()
                               .WithErrorMessage("Validation error.")
                               .AddError(new("Id", ["Not found."]));
        }

        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<bool>> HandleAsync(TaskItemUpdateStatusCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var data = await repository.GetByIdAsync(command.Id, ct);
        data.Status = command.Payload;
        await repository.ExecuteUpdateAsync(x => x.Id == command.Id, data, ct);
        var count = await repository.SaveChangesAsync(ct);

        return Result<bool>.Success()
                           .WithPayload(count > 0)
                           .WithMessage(count > 0 ? "Updated successfully." : "Update failed.");
    }
}