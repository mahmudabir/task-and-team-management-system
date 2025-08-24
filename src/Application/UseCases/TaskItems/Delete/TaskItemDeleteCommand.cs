using System.ComponentModel;

using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models.Command;

namespace Application.UseCases.TaskItems.Delete;

[Group("Country")]
[Description("Delete country by Id")]
public sealed class TaskItemDeleteCommand(long id) : Command<long>(id);