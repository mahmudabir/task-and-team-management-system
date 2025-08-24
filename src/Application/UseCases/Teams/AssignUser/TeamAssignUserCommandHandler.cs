using Application.Mappers.Teams;

using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Users;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Shared.Models.Teams;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.Update;

[ScopedLifetime]
public class TeamAssignUserCommandCommandHandler(IValidator<TeamAssignUserCommand> validator, ITeamRepository repository, IUserRepository userRepository) : CommandHandler<TeamAssignUserCommand, bool>
{
    public override async Task<Result<bool>> ValidateAsync(TeamAssignUserCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<bool>.Error()
                               .WithErrorMessage("Validation error.")
                               .WithErrors(validationResult.ToDictionary());
        }


        if (!await repository.ExistsByAsync(x => x.Id == command.Id, false, ct))
        {
            return Result<bool>.Error()
                               .WithErrorMessage("Validation error.")
                               .AddError(new("Id", ["Not found."]));
        }

        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<bool>> HandleAsync(TeamAssignUserCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var users = await userRepository.GetAllAsync(x => command.Payload.Contains(x.Id), asNoTracking: false, cancellationToken: ct);

        users.ForEach(user =>
        {
            user.TeamId = command.Id;
        });
        await repository.SaveChangesAsync(ct);

        return Result<bool>.Success()
                           .WithPayload(true)
                           .WithMessage("Assigned successfully.");
    }
}