using System.ComponentModel;

using Shared.Pagination;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Abstraction;

namespace Application.UseCases.TaskItems.Get;

[Group("TaskItem")]
[Description("Search TaskItem query")]
public class TaskItemGetQuery : IQuery
{
    public string? Q { get; set; }

    public Pageable Pageable { get; set; }
    public Sortable Sortable { get; set; }
}