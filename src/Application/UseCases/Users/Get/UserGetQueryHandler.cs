using System.Linq.Expressions;

using Application.UseCases.Teams.Get;

using Domain.Abstractions.Database.Repositories;
using Domain.Entities.Users;

using Microsoft.EntityFrameworkCore;

using Shared.Models.Users;
using Shared.Pagination;

using Softoverse.CqrsKit.Abstractions.Handlers;
using Softoverse.CqrsKit.Attributes;
using Softoverse.CqrsKit.Models;
using Softoverse.CqrsKit.Models.Utility;

namespace Application.UseCases.Users.Get;

[ScopedLifetime]
public class UserGetQueryHandler(IUserRepository userRepository) : QueryHandler<UserGetQuery, PagedData<User>>
{
    public override async Task<Result<PagedData<User>>> HandleAsync(UserGetQuery query, CqrsContext context, CancellationToken ct = default)
    {
        Expression<Func<ApplicationUser, bool>> predicate = (x) => string.IsNullOrEmpty(query.Q) || (EF.Functions.Like(x.UserName, $"%{query.Q}%") ||
                                                                                                    EF.Functions.Like(x.Email, $"%{query.Q}%") ||
                                                                                                    EF.Functions.Like(x.PhoneNumber, $"%{query.Q}%"));

        var data = await userRepository.GetAllPagedAsync<User>(predicate, query.Pageable, query.Sortable, true, ct);

        var result = Result<PagedData<User>>.Create(data.Content.Count() > 0)
                                       .WithPayload(data)
                                       .WithMessageLogic(x => x.Payload.Content.Count() > 0)
                                       .WithSuccessMessage("Data found")
                                       .WithErrorMessage("Data not found");

        return result;
    }
}