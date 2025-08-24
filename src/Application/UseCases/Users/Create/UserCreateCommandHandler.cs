using Domain.Abstractions.Database;
using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Teams;
using Domain.Entities.Users;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Shared.Models.Users;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.Create;

[ScopedLifetime]
public class ApplicationUserCreateCommandHandler(
    IValidator<UserCreateCommand> validator,
    UserManager<ApplicationUser> userManager) : CommandHandler<UserCreateCommand, User>
{
    public override async Task<Result<User>> ValidateAsync(UserCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<User>.Error()
                               .WithErrorMessage("Validation error.")
                               .WithErrors(validationResult.ToDictionary());
        }

        // if (await repository.ExistsByAsync(x =>
        //                                        x.CountryId == command.Payload.CountryId &&
        //                                        (x.NameEn == command.Payload.NameEn || x.NameBn == command.Payload.NameBn ||
        //                                         x.NameAr == command.Payload.NameAr || x.NameHi == command.Payload.NameHi),
        //                                    true,
        //                                    ct))
        // {
        //     var errors = TeamErrors.AlreadyAdded(command.Payload.NameEn);
        //
        //     return Result<Team>.Error(errors)
        //                        .WithErrorMessage("Validation error.")
        //                        .WithErrors(validationResult.ToDictionary());
        // }

        return Result<User>.Success();
    }

    public override async Task<Result<User>> HandleAsync(UserCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(command.Payload.Password))
        {
            return Result<User>.Error()
                                  .WithMessage("Validation failure")
                                  .WithError(new KeyValuePair<string, string[]>("password", ["Password field is required"]));
        }

        ApplicationUser identityUser = new ApplicationUser
        {
            UserName = command.Payload.Username,
            Email = command.Payload.Email,
            PhoneNumber = command.Payload.PhoneNumber,
        };

        command.Payload.Roles ??= [];

        var userResult = await userManager.CreateAsync(identityUser, command.Payload.Password);

        if (userResult.Succeeded)
        {
            command.Payload.Roles = command.Payload.Roles.Distinct().ToList();
            command.Payload.Roles.Remove("Admin");

            var addToRolesResult = await userManager.AddToRolesAsync(identityUser, command.Payload.Roles);
        }

        var result = Result<User>.Create(userResult.Succeeded)
                                 .WithPayload(new User
                                 {
                                     Username = command.Payload.Username,
                                     Email = command.Payload.Email
                                 })
                                 .WithMessageLogic(_ => userResult.Succeeded)
                                 .WithErrorMessage("User creation failed")
                                 .WithSuccessMessage("User created");

        return result;
    }
}