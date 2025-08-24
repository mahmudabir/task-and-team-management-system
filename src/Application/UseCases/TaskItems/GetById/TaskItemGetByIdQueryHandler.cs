using Application.Mappers.TaskItems;

using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.TaskItems;

using Shared.Models.TaskItems;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.GetById;

[ScopedLifetime]
public class TaskItemGetByIdQueryHandler(ITaskItemRepository repository, IHttpContextService httpContextService) : QueryHandler<TaskItemGetByIdQuery, TaskItemViewModel>
{
    public override async Task<Result<TaskItemViewModel>> HandleAsync(TaskItemGetByIdQuery query, CqrsContext context, CancellationToken ct = default)
    {
        var data = await repository.GetByIdAsync(query.Id, ct);

        if (data is not null)
        {
            return Result<TaskItemViewModel>.Success()
                                           .WithPayload(data.ToTaskItemViewModel())
                                           .WithSuccessMessage("Found data");
        }

        return Result<TaskItemViewModel>.Error(TaskItemErrors.NotFound(query.Id));
    }
}