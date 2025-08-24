using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Teams;

using FluentValidation;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.Update;

[ScopedLifetime]
public class TeamUpdateCommandHandler(IValidator<TeamUpdateCommand> validator, ITeamRepository repository) : CommandHandler<TeamUpdateCommand, Team>
{
    public override async Task<Result<Team>> ValidateAsync(TeamUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        if (!await repository.ExistsByAsync(x => x.Id == command.Id, false, ct))
        {
            return Result<Team>.Error()
                               .WithErrorMessage("Validation error.")
                               .AddError(new("Id", ["Not found."]));
        }

        var validationResult = await validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            return Result<Team>.Error()
                               .WithErrorMessage("Validation error.")
                               .WithErrors(validationResult.ToDictionary());
        }

        // if (await repository.ExistsByAsync(x =>
        //                                        x.Id != command.Id &&
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

        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<Team>> HandleAsync(TeamUpdateCommand command, CqrsContext context, CancellationToken ct = default)
    {
        await repository.ExecuteUpdateAsync(x => x.Id == command.Id, command.Payload, ct);
        await repository.SaveChangesAsync(ct);

        return Result<Team>.Success()
                           .WithPayload(command.Payload)
                           .WithMessage("Updated successfully.");
    }
}