using Domain.Abstractions.Database.Repositories;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Teams.Delete;

[ScopedLifetime]
public class TeamDeleteCommandHandler(ITeamRepository repository) : CommandHandler<TeamDeleteCommand, bool>
{
    public override async Task<Result<bool>> ValidateAsync(TeamDeleteCommand command, CqrsContext context, CancellationToken ct = default)
    {
        if (!await repository.ExistsByAsync(x => x.Id == command.Payload, false, ct))
        {
            return Result<bool>.Error()
                               .WithErrorMessage("Validation error.")
                               .AddError(new("Id", ["Not found."]));
        }

        return await base.ValidateAsync(command, context, ct);
    }

    public override async Task<Result<bool>> HandleAsync(TeamDeleteCommand command, CqrsContext context, CancellationToken ct = default)
    {
        await repository.ExecuteDeleteAsync(x => x.Id == command.Payload, ct);
        await repository.SaveChangesAsync(ct);
        return Result<bool>.Success()
                           .WithPayload(true)
                           .WithMessage("Deleted successfully.");
    }
}