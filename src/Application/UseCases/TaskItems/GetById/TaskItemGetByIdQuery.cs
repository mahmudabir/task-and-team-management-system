using System.ComponentModel;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Abstraction;

namespace Application.UseCases.TaskItems.GetById;

[Group("TaskItem")]
[Description("Get TaskItem by Id query")]
public class TaskItemGetByIdQuery : IQuery
{
    public long Id { get; set; }
}