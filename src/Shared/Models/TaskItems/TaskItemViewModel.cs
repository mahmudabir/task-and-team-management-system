namespace Shared.Models.TaskItems;

public class TaskItemViewModel
{
    public long Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public TaskStatus Status { get; set; }

    public DateOnly DueDate { get; set; }

    public string AssignedToUserId { get; set; }

    public string CreatedByUserId { get; set; }

    public string TeamId { get; set; }
}