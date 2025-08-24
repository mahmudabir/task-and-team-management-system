using Application.Mappers.Teams;

using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.Teams;

using Shared.Models.Teams;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.GetById;

[ScopedLifetime]
public class TeamGetByIdQueryHandler(ITeamRepository repository, IHttpContextService httpContextService) : QueryHandler<TeamGetByIdQuery, TeamViewModel>
{
    public override async Task<Result<TeamViewModel>> HandleAsync(TeamGetByIdQuery query, CqrsContext context, CancellationToken ct = default)
    {
        var data = await repository.GetByIdAsync(query.Id, ct);

        if (data is not null)
        {
            return Result<TeamViewModel>.Success()
                                        .WithPayload(data.ToTeamViewModel())
                                        .WithSuccessMessage("Found data");
        }

        return Result<TeamViewModel>.Error(TeamErrors.NotFound(query.Id));
    }
}