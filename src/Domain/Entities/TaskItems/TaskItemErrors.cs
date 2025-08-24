using Softoverse.CqrsKit.Models.Utility;

namespace Domain.Entities.TaskItems;

public static class TaskItemErrors
{
    public static CqrsError AlreadyAdded(string name) => CqrsError.Create("TaskItem.AlreadyAdded",
                                                                           $"The TaskItem with Name = '{name}' is already added.");

    public static CqrsError NotFound(long id) => CqrsError.Create("TaskItem.NotFound",
                                                                  $"The TaskItem with the Id = '{id}' was not found");
}
