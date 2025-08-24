using System.ComponentModel;

using Domain.Entities.TaskItems;
using Domain.Entities.Teams;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.TaskItems.Update;

[Group("TaskItem")]
[Description("Update TaskItem by Id")]
public sealed class TaskItemUpdateCommand(TaskItem payload) : Command<TaskItem>(payload)
{
    public long Id { get; init; }
}