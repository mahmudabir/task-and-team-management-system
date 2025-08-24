using System.ComponentModel;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.Teams.Delete;

[Group("User")]
[Description("Deactivate User by username")]
public sealed class UserReactivateCommand(string username) : Command<string>(username);