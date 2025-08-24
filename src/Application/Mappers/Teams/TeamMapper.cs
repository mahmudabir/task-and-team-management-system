using Domain.Entities.Teams;

using Riok.Mapperly.Abstractions;

using Shared.Models.Teams;

namespace Application.Mappers.Teams;

[Mapper(UseDeepCloning = true)]
public static partial class TeamMapper
{
    public static partial TeamViewModel ToTeamViewModel(this Team model);

    public static partial List<TeamViewModel> ToTeamViewModels(this IEnumerable<Team> models);
}