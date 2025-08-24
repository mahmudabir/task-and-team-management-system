using Domain.Entities.Users;

using Shared.Models.Users;

using Softoverse.CqrsKit.Models;

namespace Domain.Abstractions.Database.Repositories;

public interface IRoleService
{
    Task<Result<List<ApplicationRole>>> GetRolesAsync(string query = null, CancellationToken ct = default);
    Task<Result<ApplicationRole>> GetRoleAsync(string roleId, CancellationToken ct = default);
    Task<Result<bool>> CreateRoleAsync(string roleName, CancellationToken ct = default);
    Task<Result<bool>> UpdateRoleAsync(string roleId, ApplicationRole? role, CancellationToken ct = default);
    Task<Result<bool>> DeleteRoleAsync(string roleId, CancellationToken ct = default);

    Task<Result<List<User>>> GetUsersInRoleAsync(string roleId, CancellationToken ct = default);
    Task<Result<bool>> AssignUsersToRoleAsync(string roleId, List<string> userIds, CancellationToken ct = default);
    Task<Result<bool>> AssignUserToRoleAsync(string roleId, string userId, CancellationToken ct = default);
    Task<Result<bool>> AssignRolesToUserAsync(string userId, List<string> roleIds, CancellationToken ct = default);
}