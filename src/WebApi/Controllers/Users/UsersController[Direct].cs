using System.Linq.Expressions;

using Domain.Entities.Users;

using Infrastructure.Database;
using Infrastructure.Database.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Shared.Constants;
using Shared.Extensions;
using Shared.Models.Users;

using Softoverse.CqrsKit.Models;

using WebApi.Infrastructure.Extensions;

namespace WebApi.Controllers.Users;

[ApiController]
[Route("api/v0/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersOldController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<List<User>>>> Get([FromQuery] string q = null, [FromQuery] string roleId = null, CancellationToken ct = default)
    {
        Expression<Func<ApplicationUser, bool>> predicate = (x) => string.IsNullOrEmpty(q) || (EF.Functions.Like(x.UserName, $"%{q}%") ||
                                                                                               EF.Functions.Like(x.Email, $"%{q}%") ||
                                                                                               EF.Functions.Like(x.PhoneNumber, $"%{q}%"));

        var queryable = context.Users
                               .Where(predicate);

        if (!string.IsNullOrEmpty(roleId))
        {
            queryable = queryable.Join(context.Set<IdentityUserRole<string>>(),
                                       u => u.Id,
                                       ur => ur.UserId,
                                       (u, ur) => new
                                       {
                                           User = u,
                                           UserRole = ur
                                       })
                                 .Where(x => x.UserRole.RoleId == roleId)
                                 .Select(x => x.User);
        }

        queryable = queryable.OrderBy(x => x.UserName);

        var users = await queryable.Select(_userSelector)
                                   .ToListAsync(ct);

        var result = Result<List<User>>.Create(users.Count > 0)
                                       .WithPayload(users)
                                       .WithMessageLogic(x => x.Payload?.Count > 0)
                                       .WithSuccessMessage("Data found")
                                       .WithErrorMessage("Data not found");

        return Ok(result);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<Result<User>>> GetByUsername(string username, CancellationToken ct = default)
    {
        User? user = await context.Users
                                  .Where(x => x.UserName == username)
                                  .Select(_userSelector)
                                  .FirstOrDefaultAsync(ct);

        var result = Result<User>.Create(user != null)
                                 .WithPayload(user)
                                 .WithMessageLogic(x => x.Payload != null)
                                 .WithSuccessMessage("User found")
                                 .WithErrorMessage("User not found");

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Result<User>>> CreateUser(User userRegistration, CancellationToken ct = default)
    {
        if (!TryValidateModel(userRegistration))
        {
            return Ok(Result<User>.Error()
                                  .WithMessage("Validation failure")
                                  .WithErrors(ModelState.ToFluentErrors())
                     );
        }

        if (string.IsNullOrEmpty(userRegistration.Password))
        {
            return Ok(Result<User>.Error()
                                  .WithMessage("Validation failure")
                                  .WithError(new KeyValuePair<string, string[]>("password", ["Password field is required"]))
                     );
        }

        ApplicationUser identityUser = new ApplicationUser
        {
            UserName = userRegistration.Username,
            Email = userRegistration.Email,
            PhoneNumber = userRegistration.PhoneNumber,
        };

        userRegistration.Roles ??= [];

        var userResult = await userManager.CreateAsync(identityUser, userRegistration.Password);

        if (userResult.Succeeded)
        {
            userRegistration.Roles = userRegistration.Roles.Distinct().ToList();
            userRegistration.Roles.Remove(RoleService.AdminRoleName);

            var addToRolesResult = await userManager.AddToRolesAsync(identityUser, userRegistration.Roles);
        }

        var result = Result<User>.Create(userResult.Succeeded)
                                 .WithPayload(new User
                                 {
                                     Username = userRegistration.Username,
                                     Email = userRegistration.Email
                                 })
                                 .WithMessageLogic(_ => userResult.Succeeded)
                                 .WithErrorMessage("User creation failed")
                                 .WithSuccessMessage("User created");
        return Ok(result);
    }

    [HttpPut("{username}")]
    public async Task<ActionResult<Result<User>>> UpdateUser(string username, User userRegistration, CancellationToken ct = default)
    {
        if (!TryValidateModel(userRegistration))
        {
            return Ok(Result<User>.Error()
                                  .WithMessage("Validation failure")
                                  .WithErrors(ModelState.ToFluentErrors())
                     );
        }

        var errorAdditionalProp = new KeyValuePair<string, List<string>>("error", []);

        var user = await userManager.FindByNameAsync(username);

        if (user == null)
        {
            return Ok(Result<User>.Error()
                                  .WithPayload(new User
                                  {
                                      Username = userRegistration.Username,
                                      Email = userRegistration.Email
                                  })
                                  .WithMessage("User not found"));
        }

        if (userRegistration.Username != username)
        {
            return Ok(Result<User>.Error()
                                  .WithMessage("Username cannot be changed."));
        }

        user.Email = userRegistration.Email;
        user.PhoneNumber = userRegistration.PhoneNumber;

        userRegistration.Roles ??= [];

        if (!string.IsNullOrEmpty(userRegistration.Password))
        {
            var removePasswordResult = await userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return Ok(Result<User>.Error()
                                      .WithPayload(new User
                                      {
                                          Username = userRegistration.Username,
                                          Email = userRegistration.Email
                                      })
                                      .WithMessage("Unable to update password"));
            }

            var changePasswordResult = await userManager.AddPasswordAsync(user, userRegistration.Password);

            if (!changePasswordResult.Succeeded)
            {
                return Ok(Result<User>.Error()
                                      .WithPayload(new User
                                      {
                                          Username = userRegistration.Username,
                                          Email = userRegistration.Email
                                      })
                                      .WithMessage("Unable to update password"));
            }
        }

        var userResult = await userManager.UpdateAsync(user);

        if (userResult.Succeeded)
        {
            var existingRoles = await userManager.GetRolesAsync(user);

            bool hasAdmin = existingRoles.Contains(RoleService.AdminRoleName);
            if (!hasAdmin) userRegistration.Roles.Remove(RoleService.AdminRoleName);
            else userRegistration.Roles.Add(RoleService.AdminRoleName);
            userRegistration.Roles = userRegistration.Roles.Distinct().ToList();

            var rolesToRemove = existingRoles.Where(x => !userRegistration.Roles.Contains(x));
            if (rolesToRemove.Any())
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded) errorAdditionalProp.Value.Add("Failed to remove roles");
            }

            var rolesToAdd = userRegistration.Roles.Except(existingRoles);
            if (rolesToAdd.Any())
            {
                var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded) errorAdditionalProp.Value.Add("Failed to add roles");
            }
        }

        var result = Result<User>.Create(userResult.Succeeded)
                                 .WithPayload(new User
                                 {
                                     Username = userRegistration.Username,
                                     Email = userRegistration.Email
                                 })
                                 .WithMessageLogic(_ => userResult.Succeeded)
                                 .WithErrorMessage("User updated")
                                 .WithSuccessMessage("User update failed")
                                 .AddAdditionalProperty(new(errorAdditionalProp.Key, errorAdditionalProp.Value));
        return Ok(result);
    }

    [HttpPost("deactivate/{username}")]
    public async Task<ActionResult<Result<User>>> DeleteUser(string username, CancellationToken ct = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == username, ct);

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains(RoleService.AdminRoleName))
        {
            return BadRequest(Result<User>.Error().WithMessage($"User with {RoleService.AdminRoleName} role can not be deactivated"));
        }

        var lockoutEnableResult = await userManager.SetLockoutEnabledAsync(user!, true);
        var lockoutResult = await userManager.SetLockoutEndDateAsync(user!, DateTimeOffset.MaxValue);

        var result = Result<User>.Create(lockoutResult.Succeeded)
                                 .WithMessage("Locked user");
        return Ok(result);
    }

    [HttpPost("activate/{username}")]
    public async Task<IActionResult> ReleaseLockout(string username, CancellationToken ct = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == username, ct);
        await userManager.ResetAccessFailedCountAsync(user!);
        var lockoutEnableResult = await userManager.SetLockoutEnabledAsync(user!, true);
        var lockoutReleaseResult = await userManager.SetLockoutEndDateAsync(user!, null);

        var result = Result<User>.Create(lockoutReleaseResult.Succeeded)
                                 .WithMessage("Released Lockout");
        return Ok(result);
    }

    private readonly Expression<Func<ApplicationUser, User>> _userSelector = user => new User
    {
        Id = user.Id,
        Username = user.UserName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        IsLocked = user.LockoutEnd != null,
        Roles = context.UserRoles
                       .Where(ur => ur.UserId == user.Id)
                       .Join(context.Roles,
                             ur => ur.RoleId,
                             role => role.Id,
                             (ur, role) => role.Name)
                       .ToList()!
    };
}