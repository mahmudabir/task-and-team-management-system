using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Users;

using Microsoft.Extensions.Logging;

namespace Infrastructure.Database.Repositories;

public class UserRepository(ApplicationDbContext dbContext, ILogger<RepositoryBase<ApplicationUser, string>> logger): RepositoryBase<ApplicationUser, string>(dbContext, logger),  IUserRepository
{
}