using Domain.Entities.Users;

namespace Domain.Abstractions.Database.Repositories;

public interface IUserRepository : IRepositoryBase<ApplicationUser, string>
{

}