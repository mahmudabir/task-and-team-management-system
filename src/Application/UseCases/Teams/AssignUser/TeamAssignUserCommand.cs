using System.ComponentModel;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.Teams.Update;

[Group("Team")]
[Description("Assign Users to Team by Id")]
public sealed class TeamAssignUserCommand(List<string> payload) : Command<List<string>>(payload)
{
    public long Id { get; init; }
}