using System.ComponentModel;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.Teams.Delete;

[Group("User")]
[Description("Deactivate User by username")]
public sealed class UserDeactivateCommand(string username) : Command<string>(username);