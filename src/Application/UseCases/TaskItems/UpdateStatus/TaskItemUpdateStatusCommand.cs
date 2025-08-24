using System.ComponentModel;

using Domain.Entities.TaskItems;
using Domain.Entities.Teams;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.TaskItems.Update;

[Group("TaskItem")]
[Description("Update status of TaskItem by Id")]
public sealed class TaskItemUpdateStatusCommand(TaskStatus payload) : Command<TaskStatus>(payload)
{
    public long Id { get; init; }
}