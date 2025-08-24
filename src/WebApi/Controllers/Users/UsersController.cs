using Application.UseCases.Teams.Create;
using Application.UseCases.Teams.Delete;
using Application.UseCases.Teams.Get;
using Application.UseCases.Teams.GetById;
using Application.UseCases.Teams.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Shared.Models.Users;
using Shared.Pagination;

using Softoverse.CqrsKit.Builders;
using Softoverse.CqrsKit.Models;

namespace WebApi.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController(IServiceProvider services) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<List<User>>>> Get([FromQuery] UserGetQuery query,
                                                            [FromQuery] Pageable pageable,
                                                            [FromQuery] Sortable sortable,
                                                            CancellationToken ct = default)
    {
        query = query ?? new();
        query.Pageable = pageable;
        query.Sortable = sortable;

        var executor = CqrsBuilder.Query<UserGetQuery, PagedData<User>>(services)
                                  .WithQuery(query)
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<Result<User>>> GetByUsername([FromQuery] string username,
                                                                CancellationToken ct = default)
    {
        var executor = CqrsBuilder.Query<UserGetByUsernameQuery, User>(services)
                                  .WithQuery(new UserGetByUsernameQuery
                                  {
                                      Username = username
                                  })
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Result<User>>> CreateUser(User payload, CancellationToken ct = default)
    {
        var executor = CqrsBuilder.Command<UserCreateCommand, User>(services)
                                  .WithCommand(new(payload))
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPut("{username}")]
    public async Task<ActionResult<Result<User>>> UpdateUser([FromRoute] string username, User payload, CancellationToken ct = default)
    {
        var executor = CqrsBuilder.Command<UserUpdateCommand, User>(services)
                                  .WithCommand(new(payload)
                                  {
                                      Username = username
                                  })
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPost("deactivate/{username}")]
    public async Task<ActionResult<Result<User>>> DeleteUser(string username, CancellationToken ct = default)
    {
        var executor = CqrsBuilder.Command<UserDeactivateCommand, bool>(services)
                                  .WithCommand(new(username))
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPost("activate/{username}")]
    public async Task<IActionResult> ReleaseLockout(string username, CancellationToken ct = default)
    {
        var executor = CqrsBuilder.Command<UserReactivateCommand, bool>(services)
                                  .WithCommand(new(username))
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }
}