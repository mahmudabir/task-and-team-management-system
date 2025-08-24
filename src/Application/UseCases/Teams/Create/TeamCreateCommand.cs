using System.ComponentModel;

using Domain.Entities.Teams;

using Shared.Models.Teams;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.Teams.Create;

[Group("Team")]
[Description("Create Team")]
public sealed class TeamCreateCommand(TeamViewModel payload) : Command<TeamViewModel>(payload);