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
public class UserReactivateCommandHandler(UserManager<ApplicationUser> userManager) : CommandHandler<UserReactivateCommand, bool>
{
    public override async Task<Result<bool>> HandleAsync(UserReactivateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == command.Payload, ct);
        await userManager.ResetAccessFailedCountAsync(user!);
        var lockoutEnableResult = await userManager.SetLockoutEnabledAsync(user!, true);
        var lockoutReleaseResult = await userManager.SetLockoutEndDateAsync(user!, null);

        var result = Result<bool>.Create(lockoutReleaseResult.Succeeded)
                                 .WithMessage("Released Lockout");

        return result;
    }
}