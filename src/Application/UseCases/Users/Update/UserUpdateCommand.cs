using System.ComponentModel;

using Domain.Entities.Teams;

using Shared.Models.Users;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.Teams.Update;

[Group("User")]
[Description("Update User by Id")]
public sealed class UserUpdateCommand(User payload) : Command<User>(payload)
{
    public string Username { get; init; }
}