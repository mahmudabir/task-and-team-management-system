using Application.Mappers.Teams;

using Domain.Abstractions.Database;
using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Teams;

using FluentValidation;

using Shared.Models.Teams;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.Create;

[ScopedLifetime]
public class TeamCreateCommandHandler(IValidator<TeamCreateCommand> validator, IUnitOfWork unitOfWork, ITeamRepository repository) : CommandHandler<TeamCreateCommand, TeamViewModel>
{
    public override async Task<Result<TeamViewModel>> ValidateAsync(TeamCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<TeamViewModel>.Error()
                                        .WithErrorMessage("Validation error.")
                                        .WithErrors(validationResult.ToDictionary());
        }

        return Result<TeamViewModel>.Success();
    }

    public override async Task<Result<TeamViewModel>> HandleAsync(TeamCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var team = command.Payload.ToTeam();
        await repository.AddAsync(team, ct);
        await repository.SaveChangesAsync(ct);
        return Result<TeamViewModel>.Success()
                                    .WithPayload(team.ToTeamViewModel())
                                    .WithMessage("Added successfully.");
    }
}