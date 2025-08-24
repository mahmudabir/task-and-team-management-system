using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Users;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using Shared.Models.Users;

using Softoverse.CqrsKit.Models;

namespace Infrastructure.Database.Users;

public class RoleService : IRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoleService> _logger;

    public RoleService(UserManager<ApplicationUser> userManager,
                       RoleManager<ApplicationRole> roleManager,
                       ApplicationDbContext context,
                       ILogger<RoleService> logger,
                       IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;

        if (string.IsNullOrEmpty(AdminRoleId))
        {
            AdminRoleId = roleManager.Roles.FirstOrDefault(x => x.NormalizedName == AdminNormalizedRoleName)?.Id;
        }
    }

    public static readonly string AdminRoleName = "Admin";
    public static readonly string AdminNormalizedRoleName = "ADMIN";
    public static string AdminRoleId = ""; // It is assigned on application startup

    public static readonly string ManagerRoleName = "Manager";
    public static readonly string ManagerNormalizedRoleName = "MANAGER";
    public static string ManagerRoleId = ""; // It is assigned on application startup

    public static readonly string EmployeeRoleName = "Employee";
    public static readonly string EmployeeNormalizedRoleName = "EMPLOYEE";
    public static string EmployeeRoleId = ""; // It is assigned on application startup

    public async Task<Result<List<ApplicationRole>>> GetRolesAsync(string query = null, CancellationToken ct = default)
    {
        Expression<Func<ApplicationRole, bool>> predicate = (x) => string.IsNullOrEmpty(query)
            ? true
            : EF.Functions.Like(x.Name, $"%{query}%");

        List<ApplicationRole> roles = await _roleManager.Roles.Where(predicate).OrderBy(x => x.Name).ToListAsync(ct);
        return Result<List<ApplicationRole>>.Create(roles.Count > 0)
                                            .WithPayload(roles);
    }

    public async Task<Result<ApplicationRole>> GetRoleAsync(string roleId, CancellationToken ct = default)
    {
        var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId, ct);

        return Result<ApplicationRole>.Create(role != null)
                                      .WithPayload(role!);
    }

    public async Task<Result<bool>> CreateRoleAsync(string roleName, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(roleName) || string.IsNullOrWhiteSpace(roleName))
        {
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithError(new KeyValuePair<string, string[]>("roleName", ["'roleName' is required"]))
                               .WithMessage($"Role name is required");
        }

        if (roleName.Equals(AdminNormalizedRoleName, StringComparison.CurrentCultureIgnoreCase))
        {
            return Result<bool>.Error()
                               .WithError(new KeyValuePair<string, string[]>("role", [$"Can not create {AdminRoleName} role"]))
                               .WithPayload(false)
                               .WithMessage($"Can not create {AdminRoleName} role");
        }

        var regex = new Regex("^[a-zA-Z]+$");
        if (!regex.IsMatch(roleName))
        {
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage("Role name must contain only English alphabet");
        }

        var result = await _roleManager.CreateAsync(new ApplicationRole
        {
            Name = roleName
        });

        return Result<bool>.Create(result.Succeeded)
                           .WithPayload(result.Succeeded)
                           .WithMessageLogic(x => x.Payload)
                           .WithSuccessMessage("Role created")
                           .WithErrorMessage("Failed to create role");
    }

    public async Task<Result<bool>> UpdateRoleAsync(string roleId, ApplicationRole? role, CancellationToken ct = default)
    {
        if (role == null)
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage("Invalid data");

        if (role.Name!.Equals(AdminNormalizedRoleName, StringComparison.CurrentCultureIgnoreCase))
        {
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage($"Can not update to {AdminRoleName} role");
        }

        var roleFromDbResult = await GetRoleAsync(roleId, ct);
        if (roleFromDbResult.IsFailure)
        {
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage(roleFromDbResult.Message!);
        }

        var roleFromDb = roleFromDbResult.Payload;

        if (roleFromDb!.NormalizedName == AdminNormalizedRoleName)
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage("Can not update this role");

        roleFromDb.Name = role.Name;

        var result = await _roleManager.UpdateAsync(roleFromDb);
        return Result<bool>.Create(result.Succeeded)
                           .WithPayload(result.Succeeded)
                           .WithMessageLogic(x => x.Payload)
                           .WithSuccessMessage("Role updated")
                           .WithErrorMessage("Failed to update role");
    }

    public async Task<Result<bool>> DeleteRoleAsync(string roleId, CancellationToken ct = default)
    {
        var roleFromDbResult = await GetRoleAsync(roleId, ct);
        if (roleFromDbResult.IsFailure)
        {
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage(roleFromDbResult.Message!);
        }

        var roleFromDb = roleFromDbResult.Payload;

        if (roleFromDb == null)
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage("Role not found");

        if (roleFromDb.NormalizedName == AdminNormalizedRoleName)
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage("Can not update this role");

        if (roleFromDb.NormalizedName == AdminNormalizedRoleName)
            return Result<bool>.Error()
                               .WithPayload(false)
                               .WithMessage("Can not delete this role");

        var result = await _roleManager.DeleteAsync(roleFromDb);
        return Result<bool>.Create(result.Succeeded)
                           .WithPayload(result.Succeeded)
                           .WithMessageLogic(x => x.Payload)
                           .WithSuccessMessage("Role deleted")
                           .WithErrorMessage("Failed to delete role");
    }

    public async Task<Result<List<User>>> GetUsersInRoleAsync(string roleId, CancellationToken ct = default)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
            return Result<List<User>>.Error()
                                     .WithPayload([])
                                     .WithMessage("Role not found");

        var applicationUsersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

        var usersInRole = applicationUsersInRole.OrderBy(x => x.UserName)
                                                .Select(x => new User
                                                {
                                                    Id = x.Id,
                                                    Username = x.UserName,
                                                    Email = x.Email
                                                }).ToList();

        return Result<List<User>>.Create(usersInRole.Count > 0)
                                 .WithPayload(usersInRole);
    }

    public async Task<Result<bool>> AssignUsersToRoleAsync(string roleId, List<string> userIds, CancellationToken ct = default)
    {
        var roleFromDbResult = await GetRoleAsync(roleId, ct);
        if (roleFromDbResult.IsFailure)
        {
            return Result<bool>.Error()
                               .WithError(new KeyValuePair<string, string[]>("role", [roleFromDbResult.Message!]))
                               .WithPayload(false)
                               .WithMessage(roleFromDbResult.Message!);
        }

        var roleFromDb = roleFromDbResult.Payload;

        if (roleFromDb!.NormalizedName == AdminNormalizedRoleName)
            return Result<bool>.Error()
                               .WithError(new KeyValuePair<string, string[]>(AdminRoleName, [$"Can not assign users to {AdminRoleName} role"]))
                               .WithPayload(false)
                               .WithMessage($"Can not assign users to {AdminRoleName} role");

        // Get currently assigned users
        var currentUserRoles = await _context.UserRoles
                                             .Where(ur => ur.RoleId == roleId)
                                             .ToListAsync(ct);

        var currentUserIds = currentUserRoles.Select(ur => ur.UserId).ToHashSet();
        var requestedUserIds = userIds.ToHashSet();

        // Users to remove (present in DB but not in the request)
        var usersToRemove = currentUserRoles.Where(ur => !requestedUserIds.Contains(ur.UserId)).ToList();
        if (usersToRemove.Count != 0)
        {
            _context.UserRoles.RemoveRange(usersToRemove);
        }

        // Users to add (present in request but not in DB)
        var usersToAdd = requestedUserIds.Except(currentUserIds).ToList();
        foreach (var userId in usersToAdd)
        {
            var userExists = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
            if (!userExists)
            {
                await _context.UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    UserId = userId,
                    RoleId = roleId
                }, ct);
            }
        }

        var changeCount = await _context.SaveChangesAsync(ct);

        return Result<bool>.Success()
                           .WithPayload(true)
                           .WithMessage("Users assigned to role");
    }

    public async Task<Result<bool>> AssignUserToRoleAsync(string roleId, string userId, CancellationToken ct = default)
    {
        var roleFromDbResult = await GetRoleAsync(roleId, ct);
        if (roleFromDbResult.IsFailure)
        {
            return Result<bool>.Error()
                               .WithError(new KeyValuePair<string, string[]>("role", [roleFromDbResult.Message!]))
                               .WithPayload(false)
                               .WithMessage(roleFromDbResult.Message!);
        }

        var roleFromDb = roleFromDbResult.Payload;

        if (roleFromDb!.NormalizedName == AdminNormalizedRoleName)
            return Result<bool>.Error()
                               .WithError(new KeyValuePair<string, string[]>(AdminRoleName, [$"Can not assign users to {AdminRoleName} role"]))
                               .WithPayload(false)
                               .WithMessage($"Can not assign users to {AdminRoleName} role");

        // Get currently assigned users
        var currentUserRoles = await _context.UserRoles
                                             .Where(ur => ur.RoleId == roleId)
                                             .ToListAsync(ct);

        var currentUserIds = currentUserRoles.Select(ur => ur.UserId).ToHashSet();

        if (currentUserIds.Contains(userId))
        {
            return Result<bool>.Success()
                               .WithPayload(true)
                               .WithMessage($"User {userId} already assigned to role");
        }

        var userExists = await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
        if (!userExists)
        {
            await _context.UserRoles.AddAsync(new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            }, ct);
        }

        var changeCount = await _context.SaveChangesAsync(ct);

        return Result<bool>.Success()
                           .WithPayload(true)
                           .WithMessage("Users assigned to role");
    }

    public async Task<Result<bool>> AssignRolesToUserAsync(string userId, List<string> roleIds, CancellationToken ct = default)
    {
        var userFromDb = await _context.Users.FindAsync([userId], ct);
        if (userFromDb == null)
        {
            return Result<bool>.Error()
                               .WithError(new KeyValuePair<string, string[]>("user", ["User not found"]))
                               .WithPayload(false)
                               .WithMessage("User not found");
        }

        // Get currently assigned roles
        var currentUserRoles = await _context.UserRoles
                                             .Where(ur => ur.UserId == userId)
                                             .ToListAsync(ct);

        ApplicationRole? admin = await _context.Set<ApplicationRole>().FirstOrDefaultAsync(x => x.NormalizedName == AdminNormalizedRoleName, ct);
        var hasAdmin = currentUserRoles.Any(x => x.RoleId == admin.Id)!;

        var currentRoleIds = currentUserRoles.Select(ur => ur.RoleId).ToHashSet();

        var result = Result<bool>.Success().WithPayload(true).WithMessage("Roles assigned to user");

        if (hasAdmin)
        {
            if (!roleIds.Contains(admin.Id))
            {
                result.AddAdditionalProperty(new KeyValuePair<string, object>("error", new[]
                {
                    $"Can not unassign user from {AdminRoleName} role"
                }));

                roleIds.Add(admin.Id);
            }
        }
        else
        {
            if (roleIds.Contains(AdminRoleId))
            {
                result.AddAdditionalProperty(new KeyValuePair<string, object>("error", new[]
                {
                    $"Can not assign user to {AdminRoleName} role"
                }));

                roleIds.Remove(AdminRoleId);
            }
        }

        var requestedRoleIds = roleIds.ToHashSet();

        // Roles to remove (present in DB but not in the request)
        var rolesToRemove = currentUserRoles.Where(ur => ur.UserId == userId && !requestedRoleIds.Contains(ur.RoleId)).ToList();
        if (rolesToRemove.Count != 0)
        {
            _context.UserRoles.RemoveRange(rolesToRemove);
        }

        // Roles to add (present in request but not in DB)
        var rolesToAdd = requestedRoleIds.Except(currentRoleIds).ToList();
        foreach (var roleId in rolesToAdd)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId, ct);
            if (roleExists)
            {
                await _context.UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    UserId = userId,
                    RoleId = roleId
                }, ct);
            }
        }

        var changeCount = await _context.SaveChangesAsync(ct);

        return result;
    }
}