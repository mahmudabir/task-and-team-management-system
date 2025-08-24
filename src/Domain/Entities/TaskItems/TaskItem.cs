using Domain.Entities.Teams;
using Domain.Entities.Users;

using Shared.Enums;

namespace Domain.Entities.TaskItems;

public class TaskItem : Entity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskItemStatus Status { get; set; }

    public DateTime DueDate { get; set; }

    public string AssignedToUserId { get; set; }
    public ApplicationUser? AssignedToUser { get; set; }

    public string CreatedByUserId { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }

    public long TeamId { get; set; }
    public Team? Team { get; set; }
}