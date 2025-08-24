using System.ComponentModel;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.Teams.Delete;

[Group("Team")]
[Description("Delete Team by Id")]
public sealed class TeamDeleteCommand(long id) : Command<long>(id);