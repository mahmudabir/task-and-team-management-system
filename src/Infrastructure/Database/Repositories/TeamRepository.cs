using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Teams;

using Microsoft.Extensions.Logging;

namespace Infrastructure.Database.Repositories;

public class TeamRepository(ApplicationDbContext dbContext, ILogger<TeamRepository> logger)
    : GenericRepository<Team, long>(dbContext, logger),
      ITeamRepository;