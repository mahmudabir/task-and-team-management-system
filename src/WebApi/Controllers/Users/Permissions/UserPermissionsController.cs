using Domain.Abstractions.Database.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Shared.Models.Users;

using Softoverse.CqrsKit.Models;

namespace WebApi.Controllers.Users.Permissions;

[ApiController]
[Route("api/permissions")]
[Authorize(Roles = "Admin")]
public partial class PermissionsController(IRoleService roleService, IServiceProvider services) : ControllerBase
{
    [HttpGet("roles/{roleId}/users")]
    public async Task<ActionResult<Result<List<User>>>> GetUsersInRole(string roleId, CancellationToken ct = default)
    {
        var result = await roleService.GetUsersInRoleAsync(roleId, ct);
        return Ok(result);
    }

    [HttpPost("roles/{roleId}/users")]
    public async Task<ActionResult<Result<bool>>> AssignUsersToRole(string roleId, [FromBody] List<string> userIds, CancellationToken ct = default)
    {
        var result = await roleService.AssignUsersToRoleAsync(roleId, userIds, ct);
        return Ok(result);
    }

    [HttpPost("roles/{roleId}/users/{userId}")]
    public async Task<ActionResult<Result<bool>>> AssignUserToRole(string roleId, string userId, CancellationToken ct = default)
    {
        var result = await roleService.AssignUserToRoleAsync(roleId, userId, ct);
        return Ok(result);
    }

    [HttpPost("users/{userId}")]
    public async Task<ActionResult<Result<bool>>> AssignUserToRole(string userId, List<string> roleIds, CancellationToken ct = default)
    {
        var result = await roleService.AssignRolesToUserAsync(userId, roleIds, ct);
        return Ok(result);
    }
}