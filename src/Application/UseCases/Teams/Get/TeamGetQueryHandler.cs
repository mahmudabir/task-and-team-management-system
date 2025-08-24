using Application.Mappers.Teams;

using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.Teams;

using Shared.Models.Teams;
using Shared.Pagination;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.Get;

[ScopedLifetime]
public class TeamGetQueryHandler(ITeamRepository repository, IHttpContextService httpContextService) : QueryHandler<TeamGetQuery, PagedData<TeamViewModel>>
{
    public override async Task<Result<PagedData<TeamViewModel>>> HandleAsync(TeamGetQuery query, CqrsContext context, CancellationToken ct = default)
    {
        PagedData<Team> data = await repository.GetAllPagedAsync(x=>true, query.Pageable, query.Sortable, true, ct);

        var payload = new PagedData<TeamViewModel>(data)
        {
            Content = data.Content.ToTeamViewModels()
        };

        return Result<PagedData<TeamViewModel>>.Success()
                                               .WithPayload(payload)
                                               .WithMessageLogic(x => x.Payload?.Content?.Count() > 0)
                                               .WithSuccessMessage("Found data")
                                               .WithErrorMessage("No data found");
    }
}