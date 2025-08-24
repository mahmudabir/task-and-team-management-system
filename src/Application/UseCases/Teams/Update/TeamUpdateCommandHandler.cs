using Application.Mappers.Teams;

using Domain.Abstractions.Database.Repositories;

using FluentValidation;

using Shared.Models.Teams;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.Update;

[ScopedLifetime]
public class TeamUpdateCommandHandler(IValidator<TeamUpdateCommand> validator, ITeamRepository repository) : CommandHandler<TeamUpdateCommand, TeamViewModel>
{
    public override async Task<Result<TeamViewModel>> ValidateAsync(TeamUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        if (!await repository.ExistsByAsync(x => x.Id == command.Id, false, ct))
        {
            return Result<TeamViewModel>.Error()
                                        .WithErrorMessage("Validation error.")
                                        .AddError(new("Id", ["Not found."]));
        }

        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<TeamViewModel>.Error()
                                        .WithErrorMessage("Validation error.")
                                        .WithErrors(validationResult.ToDictionary());
        }

        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<TeamViewModel>> HandleAsync(TeamUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var team = command.Payload.ToTeam();
        await repository.ExecuteUpdateAsync(x => x.Id == command.Id, team, ct);
        await repository.SaveChangesAsync(ct);

        return Result<TeamViewModel>.Success()
                                    .WithPayload(team.ToTeamViewModel())
                                    .WithMessage("Updated successfully.");
    }
}