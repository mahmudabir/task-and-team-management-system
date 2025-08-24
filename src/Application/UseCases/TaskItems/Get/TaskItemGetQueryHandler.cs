using System.Linq.Expressions;

using Application.Mappers.TaskItems;

using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.TaskItems;
using Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;

using Shared.Models.TaskItems;
using Shared.Pagination;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Get;

[ScopedLifetime]
public class TaskItemGetQueryHandler(ITaskItemRepository repository, UserManager<ApplicationUser> userManager, IHttpContextService httpContextService) : QueryHandler<TaskItemGetQuery, PagedData<TaskItemViewModel>>
{
    public override async Task<Result<PagedData<TaskItemViewModel>>> HandleAsync(TaskItemGetQuery query, CqrsContext context, CancellationToken ct = default)
    {
        var roles = httpContextService.GetCurrentUserRoles();
        var userId = httpContextService.GetCurrentUserIdentity();
        Expression<Func<TaskItem, bool>> predicate = x => (query.Status == null) || (x.Status == query.Status)
                                                                                 || (query.AssignedTo == null) || x.AssignedToUser.UserName == query.AssignedTo
                                                                                 || (query.TeamName == null) || x.Team.Name == query.TeamName
                                                                                 || (query.DueDate == null) || x.DueDate == query.DueDate;

        PagedData<TaskItem> data = null;

        if (roles.Contains("Admin"))
        {
            data = await repository.GetAllPagedAsync<TaskItem>(predicate, query.Pageable, query.Sortable, true, ct);
        }
        else if (roles.Contains("Manager"))
        {
            var manager = await userManager.FindByIdAsync(userId);
            predicate = x => x.TeamId == manager.TeamId
                          || (query.Status == null) || (x.Status == query.Status)
                          || (query.AssignedTo == null) || (x.AssignedToUserId == query.AssignedTo)
                          || (query.DueDate == null) || x.DueDate == query.DueDate;

            data = await repository.GetAllPagedAsync<TaskItem>(predicate, query.Pageable, query.Sortable, true, ct);
        }
        else if (roles.Contains("Employee"))
        {
            var employee = await userManager.FindByIdAsync(userId);
            predicate = x => x.TeamId == employee.TeamId && x.AssignedToUserId == employee.Id
                          || (query.Status == null) || (x.Status == query.Status)
                          || (query.DueDate == null) || x.DueDate == query.DueDate;

            data = await repository.GetAllPagedAsync<TaskItem>(predicate, query.Pageable, query.Sortable, true, ct);
        }

        PagedData<TaskItemViewModel> payload = new(data)
        {
            Content = data.Content.ToTaskItemViewModels(),
        };

        return Result<PagedData<TaskItemViewModel>>.Success()
                                                   .WithPayload(payload)
                                                   .WithMessageLogic(x => x.Payload?.Content?.Count() > 0)
                                                   .WithSuccessMessage("Found data")
                                                   .WithErrorMessage("No data found");
    }
}