using Domain.Abstractions.Database;
using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Teams;

using FluentValidation;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.Create;

[ScopedLifetime]
public class TeamCreateCommandHandler(IValidator<TeamCreateCommand> validator, IUnitOfWork unitOfWork, ITeamRepository repository) : CommandHandler<TeamCreateCommand, Team>
{
    public override async Task<Result<Team>> ValidateAsync(TeamCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<Team>.Error()
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

        return Result<Team>.Success();
    }

    public override async Task<Result<Team>> HandleAsync(TeamCreateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        await repository.AddAsync(command.Payload, ct);
        await repository.SaveChangesAsync(ct);
        return Result<Team>.Success()
                           .WithPayload(command.Payload)
                           .WithMessage("Added successfully.");
    }
}