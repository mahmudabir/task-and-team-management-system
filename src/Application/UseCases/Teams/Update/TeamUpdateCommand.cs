using System.ComponentModel;

using Domain.Entities.Teams;

using Shared.Models.Teams;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.Teams.Update;

[Group("Team")]
[Description("Update Team by Id")]
public sealed class TeamUpdateCommand(TeamViewModel payload) : Command<TeamViewModel>(payload)
{
    public long Id { get; init; }
}