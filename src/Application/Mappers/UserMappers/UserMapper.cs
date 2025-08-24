using Domain.Entities.Users;

using Riok.Mapperly.Abstractions;

using Shared.Models.Users;

namespace Application.Mappers.UserMappers;

[Mapper(UseDeepCloning = true)]
public static partial class UserMapper
{
    public static partial User ToUser(this ApplicationUser model);

    public static partial List<User> ToUsers(this IEnumerable<ApplicationUser> models);
}