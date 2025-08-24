using System.ComponentModel;

using Shared.Pagination;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Abstraction;

namespace Application.UseCases.Teams.Get;

[Group("Team")]
[Description("Search Teams query")]
public class TeamGetQuery : IQuery
{
    public Pageable Pageable { get; set; }
    public Sortable Sortable { get; set; }
}