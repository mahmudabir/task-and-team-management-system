using Domain.Abstractions.Database.Repositories;
using Domain.Entities.TaskItems;

using Microsoft.Extensions.Logging;

namespace Infrastructure.Database.Repositories;

public class TaskItemRepository(ApplicationDbContext dbContext, ILogger<TaskItemRepository> logger)
    : GenericRepository<TaskItem, long>(dbContext, logger),
      ITaskItemRepository;