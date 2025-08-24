using Application.Mappers.TaskItems;

using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.TaskItems;
using Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;

using Shared.Models.TaskItems;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.GetById;

[ScopedLifetime]
public class TaskItemGetByIdQueryHandler(ITaskItemRepository repository, UserManager<ApplicationUser> userManager, IHttpContextService httpContextService) : QueryHandler<TaskItemGetByIdQuery, TaskItemViewModel>
{
    public override async Task<Result<TaskItemViewModel>> HandleAsync(TaskItemGetByIdQuery query, CqrsContext context, CancellationToken ct = default)
    {
        var roles = httpContextService.GetCurrentUserRoles();
        var username = httpContextService.GetCurrentUserIdentity();

        TaskItem? data;

        if (roles.Contains("Admin"))
        {
            data = await repository.GetAsync(x => x.Id == query.Id, true, ct);
            if (data is not null)
            {
                return Result<TaskItemViewModel>.Success()
                                                .WithPayload(data.ToTaskItemViewModel())
                                                .WithSuccessMessage("Found data");
            }
        }

        if (roles.Contains("Manager"))
        {
            var manager = await userManager.FindByNameAsync(username);
            data = await repository.GetAsync(x => x.Id == query.Id && x.CreatedByUserId == manager.Id, true, ct);
            if (data is not null)
            {
                return Result<TaskItemViewModel>.Success()
                                                .WithPayload(data.ToTaskItemViewModel())
                                                .WithSuccessMessage("Found data");
            }
        }

        if (roles.Contains("Employee"))
        {
            var employee = await userManager.FindByNameAsync(username);
            data = await repository.GetAsync(x => x.Id == query.Id && x.AssignedToUserId == employee.Id, true, ct);
            if (data is not null)
            {
                return Result<TaskItemViewModel>.Success()
                                                .WithPayload(data.ToTaskItemViewModel())
                                                .WithSuccessMessage("Found data");
            }
        }

        return Result<TaskItemViewModel>.Error(TaskItemErrors.NotFound(query.Id));
    }
}