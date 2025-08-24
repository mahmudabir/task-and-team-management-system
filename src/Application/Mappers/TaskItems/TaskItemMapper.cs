using Domain.Entities.TaskItems;

using Riok.Mapperly.Abstractions;

using Shared.Models.TaskItems;

namespace Application.Mappers.TaskItems;

[Mapper(UseDeepCloning = true)]
public static partial class TaskItemMapper
{
    public static partial TaskItemViewModel ToTaskItemViewModel(this TaskItem model);

    public static partial List<TaskItemViewModel> ToTaskItemViewModels(this IEnumerable<TaskItem> models);
    public static partial TaskItem ToTaskItem(this TaskItemViewModel model);
}