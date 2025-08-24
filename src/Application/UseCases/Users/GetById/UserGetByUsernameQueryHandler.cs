using Application.Mappers.UserMappers;
using Application.UseCases.Teams.GetById;

using Domain.Abstractions.Database.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities.Users;

using Shared.Models.Users;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Users.GetById;

[ScopedLifetime]
public class UserGetByUsernameQueryHandler(IUserRepository repository, IHttpContextService httpContextService) : QueryHandler<UserGetByUsernameQuery, User>
{
    public override async Task<Result<User>> HandleAsync(UserGetByUsernameQuery query, CqrsContext context, CancellationToken ct = default)
    {
        var data = await repository.GetByIdAsync(query.Username, ct);

        if (data is not null)
        {
            return Result<User>.Success()
                                        .WithPayload(data.ToUser())
                                        .WithSuccessMessage("Found data");
        }

        return Result<User>.Error(UserErrors.NotFound(query.Username));
    }
}