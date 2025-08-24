using System.ComponentModel;

using Domain.Entities.Teams;
using Domain.Entities.Users;

using Shared.Models.Users;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.Teams.Create;

[Group("User")]
[Description("Create Team")]
public sealed class UserCreateCommand(User payload) : Command<User>(payload);