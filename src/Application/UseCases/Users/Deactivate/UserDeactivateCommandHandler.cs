using Application.UseCases.Teams.Delete;

using Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Users.Delete;

[ScopedLifetime]
public class UserDeactivateCommandHandler(UserManager<ApplicationUser> userManager) : CommandHandler<UserDeactivateCommand, bool>
{
    public override async Task<Result<bool>> ValidateAsync(UserDeactivateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        ApplicationUser? user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == command.Payload, ct);

        var roles = await userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
        {
            return Result<bool>.Error().WithMessage($"User with Admin role can not be deactivated");
        }

        context.Items.Add("User", user);
        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<bool>> HandleAsync(UserDeactivateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var user = context.GetItem<ApplicationUser>("User");
        var lockoutEnableResult = await userManager.SetLockoutEnabledAsync(user!, true);
        var lockoutResult = await userManager.SetLockoutEndDateAsync(user!, DateTimeOffset.MaxValue);

        var result = Result<bool>.Create(lockoutResult.Succeeded)
                                 .WithMessage("Locked user");
        return result;
    }
}