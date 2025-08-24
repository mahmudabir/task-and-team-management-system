using Shared.Enums;

namespace Shared.Models.TaskItems;

public class TaskItemViewModel
{
    public long Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public TaskItemStatus Status { get; set; }

    public DateTime DueDate { get; set; }

    public string AssignedToUserId { get; set; }

    public string CreatedByUserId { get; set; }

    public long TeamId { get; set; }
}