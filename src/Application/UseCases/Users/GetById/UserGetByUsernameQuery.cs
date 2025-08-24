using System.ComponentModel;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Abstraction;

namespace Application.UseCases.Teams.GetById;

[Group("User")]
[Description("Get User by Username query")]
public class UserGetByUsernameQuery : IQuery
{
    public string Username { get; set; }
}