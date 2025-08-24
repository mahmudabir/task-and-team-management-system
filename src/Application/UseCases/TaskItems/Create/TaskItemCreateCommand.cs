using System.ComponentModel;

using Domain.Entities.TaskItems;

using Shared.Models.TaskItems;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.TaskItems.Create;

[Group("TaskItem")]
[Description("Create TaskItem")]
public sealed class TaskItemCreateCommand(TaskItemViewModel payload) : Command<TaskItemViewModel>(payload);