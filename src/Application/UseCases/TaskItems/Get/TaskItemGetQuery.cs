using System.ComponentModel;

using Shared.Enums;
using Shared.Pagination;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Abstraction;

namespace Application.UseCases.TaskItems.Get;

[Group("TaskItem")]
[Description("Search TaskItem query")]
public class TaskItemGetQuery : IQuery
{
    public TaskItemStatus? Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? TeamName { get; set; }
    public DateTime? DueDate { get; set; }

    public Pageable Pageable { get; set; }
    public Sortable Sortable { get; set; }
}