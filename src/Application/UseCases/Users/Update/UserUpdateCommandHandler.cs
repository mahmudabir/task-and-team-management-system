using Application.UseCases.Teams.Update;

using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Users;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Shared.Models.Users;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Users.Update;

[ScopedLifetime]
public class UserUpdateCommandHandler(UserManager<ApplicationUser> userManager, IUserRepository repository) : CommandHandler<UserUpdateCommand, User>
{
    public override async Task<Result<User>> ValidateAsync(UserUpdateCommand command, CqrsContext context, CancellationToken ct = new CancellationToken())
    {
        var username = command.Username;
        var userRegistration = command.Payload;

        var user = await userManager.FindByIdAsync(command.Username);

        if (user == null)
        {
            Result<User>.Error()
                        .WithPayload(new User
                        {
                            Username = userRegistration.Username,
                            Email = userRegistration.Email
                        })
                        .WithMessage("User not found");
        }

        if (userRegistration.Username != username)
        {
            return Result<User>.Error()
                               .WithMessage("Username cannot be changed.");
        }

        userRegistration.Roles ??= [];

        if (!string.IsNullOrEmpty(userRegistration.Password))
        {
            var removePasswordResult = await userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return Result<User>.Error()
                                   .WithPayload(new User
                                   {
                                       Username = userRegistration.Username,
                                       Email = userRegistration.Email
                                   })
                                   .WithMessage("Unable to update password");
            }

            var changePasswordResult = await userManager.AddPasswordAsync(user, userRegistration.Password);

            if (!changePasswordResult.Succeeded)
            {
                return Result<User>.Error()
                                   .WithPayload(new User
                                   {
                                       Username = userRegistration.Username,
                                       Email = userRegistration.Email
                                   })
                                   .WithMessage("Unable to update password");
            }
        }

        context.Items.Add("User", user);

        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<User>> HandleAsync(UserUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var username = command.Username;
        var userRegistration = command.Payload;
        var errorAdditionalProp = new KeyValuePair<string, List<string>>("error", []);

        var user = context.GetItem<ApplicationUser>("User");
        user.Email = userRegistration.Email;
        user.PhoneNumber = userRegistration.PhoneNumber;
        user.FullName = userRegistration.FullName;
        user.TeamId = userRegistration.TeamId;

        var userResult = await userManager.UpdateAsync(user);

        if (userResult.Succeeded)
        {
            var existingRoles = await userManager.GetRolesAsync(user);

            bool hasAdmin = existingRoles.Contains("Admin");
            if (!hasAdmin) userRegistration.Roles.Remove("Admin");
            else userRegistration.Roles.Add("Admin");
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
        return result;
    }
}