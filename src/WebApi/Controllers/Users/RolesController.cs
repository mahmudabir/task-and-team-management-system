using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Softoverse.CqrsKit.Models;

namespace WebApi.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<List<ApplicationRole>>>> GetRoles([FromQuery] string q = null, CancellationToken ct = default)
    {
        var result = await roleService.GetRolesAsync(q, ct);
        return Ok(result);
    }
    
    [HttpGet("{roleId}")]
    public async Task<ActionResult<Result<ApplicationRole>>> GetRoleById(string roleId, CancellationToken ct = default)
    {
        var result = await roleService.GetRoleAsync(roleId, ct);
        return Ok(result);
    }
}