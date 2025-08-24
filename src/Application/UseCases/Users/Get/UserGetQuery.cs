using System.ComponentModel;

using Shared.Pagination;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Abstraction;

namespace Application.UseCases.Teams.Get;

[Group("User")]
[Description("Search Users query")]
public class UserGetQuery : IQuery
{
    public string? Q { get; set; }

    public Pageable Pageable { get; set; }
    public Sortable Sortable { get; set; }
}