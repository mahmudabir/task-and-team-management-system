using System.ComponentModel;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Abstraction;

namespace Application.UseCases.Teams.GetById;

[Group("Team")]
[Description("Get Team by Id query")]
public class TeamGetByIdQuery : IQuery
{
    public long Id { get; set; }
}