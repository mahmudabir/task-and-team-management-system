using Domain.Abstractions.Database.Repositories;

using Infrastructure.Database;
using Infrastructure.Database.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Softoverse.CqrsKit.Models;

namespace WebApi.Controllers.Users.Permissions;

[ApiController]
[Route("api/permissions")]
[Authorize(Roles = "Admin")]
public partial class PermissionsController(IRoleService roleService, IServiceProvider services) : ControllerBase
{
    [HttpPost("seed-data")]
    public async Task<ActionResult<Result<bool>>> SeedData()
    {
        try
        {
            Task.Run(async () =>
            {
                using var scope = services.CreateScope();
                await scope.SeedAdminUserData();
            });
        }
        catch (Exception ex)
        {
            return Result<bool>.Error().WithMessage("Seed data failed.");
        }

        return Result<bool>.Success().WithPayload(true).WithMessage("Seeding data in background.");
    }
}