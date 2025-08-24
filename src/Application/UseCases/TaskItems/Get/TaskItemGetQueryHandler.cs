using Application.Mappers.TaskItems;

using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.TaskItems;

using Shared.Models.TaskItems;
using Shared.Pagination;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.TaskItems.Get;

[ScopedLifetime]
public class TaskItemGetQueryHandler(ITaskItemRepository repository, IHttpContextService httpContextService) : QueryHandler<TaskItemGetQuery, PagedData<TaskItemViewModel>>
{
    public override async Task<Result<PagedData<TaskItemViewModel>>> HandleAsync(TaskItemGetQuery query, CqrsContext context, CancellationToken ct = default)
    {
        var data = await repository.GetAllPagedAsync<TaskItem>(x => true, query.Pageable, query.Sortable, true, ct);

        PagedData<TaskItemViewModel> payload = new PagedData<TaskItemViewModel>(data)
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