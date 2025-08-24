using Domain.Entities.TaskItems;
using Domain.Entities.Teams;

namespace Domain.Abstractions.Database.Repositories;

public interface ITaskItemRepository : IRepository<TaskItem, long>;