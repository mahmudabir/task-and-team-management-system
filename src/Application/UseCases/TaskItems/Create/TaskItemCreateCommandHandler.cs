using Application.Mappers.TaskItems;

using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.Users;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Shared.Models.TaskItems;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Create;

[ScopedLifetime]
public class TaskItemCreateCommandHandler(IValidator<TaskItemCreateCommand> validator, ITaskItemRepository repository, IUserRepository userRepository, IHttpContextService httpContextService, UserManager<ApplicationUser> userManager) : CommandHandler<TaskItemCreateCommand, TaskItemViewModel>
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

        // Only validate AssignedToUserId if provided
        if (!string.IsNullOrWhiteSpace(command.Payload.AssignedToUserId))
        {
            var exists = await userRepository.ExistsByAsync(x => x.Id == command.Payload.AssignedToUserId, true, ct);
            if (!exists)
            {
                return Result<TaskItemViewModel>.Error(UserErrors.NotFound(command.Payload.AssignedToUserId!))
                                                .WithErrorMessage("Validation error.");
            }
        }

        return Result<TaskItemViewModel>.Success();
    }

    public override async Task<Result<TaskItemViewModel>> HandleAsync(TaskItemCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var task = command.Payload.ToTaskItem();

        var userId = httpContextService.GetCurrentUserIdentity();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<TaskItemViewModel>.Error(UserErrors.Unauthorized());
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result<TaskItemViewModel>.Error(UserErrors.Unauthorized());
        }

        task.CreatedByUserId = user.Id;
        await repository.AddAsync(task, ct);
        await repository.SaveChangesAsync(ct);
        return Result<TaskItemViewModel>.Success()
                                        .WithPayload(task.ToTaskItemViewModel())
                                        .WithMessage("Added successfully.");
    }
}