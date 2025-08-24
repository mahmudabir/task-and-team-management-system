using Application.UseCases.Teams.Create;
using Application.UseCases.Teams.Delete;
using Application.UseCases.Teams.Get;
using Application.UseCases.Teams.GetById;
using Application.UseCases.Teams.Update;

using Domain.Entities.Teams;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Shared.Models.Teams;
using Shared.Pagination;

using Softoverse.CqrsKit.Builders;
using Softoverse.CqrsKit.Models;

namespace WebApi.Controllers.Teams;

[Route("api/[controller]")]
[ApiController]
public class TeamsController(IServiceProvider services) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<PagedData<TeamViewModel>>>> Get([FromQuery] TeamGetQuery query,
                                                                          [FromQuery] Pageable pageable,
                                                                          [FromQuery] Sortable sortable,
                                                                          CancellationToken ct = default)
    {
        query = query ?? new();
        query.Pageable = pageable;
        query.Sortable = sortable;

        var executor = CqrsBuilder.Query<TeamGetQuery, PagedData<TeamViewModel>>(services)
                                  .WithQuery(query)
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Result<TeamViewModel>>> GetById(long id,
                                                                   CancellationToken ct = default)
    {
        var executor = CqrsBuilder.Query<TeamGetByIdQuery, TeamViewModel>(services)
                                  .WithQuery(new TeamGetByIdQuery
                                  {
                                      Id = id
                                  })
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Team>> Post([FromBody] TeamViewModel payload,
                                               CancellationToken cancellationToken)
    {
        var executor = CqrsBuilder.Command<TeamCreateCommand, TeamViewModel>(services)
                                  .WithCommand(new(payload))
                                  .Build();

        var result = await executor.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<ActionResult<Team>> Put([FromRoute] long id,
                                              [FromBody] TeamViewModel payload,
                                              CancellationToken cancellationToken)
    {
        var executor = CqrsBuilder.Command<TeamUpdateCommand, Team>(services)
                                  .WithCommand(new(payload)
                                  {
                                      Id = id
                                  })
                                  .Build();

        var result = await executor.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<ActionResult<bool>> Delete([FromRoute] long id,
                                                 CancellationToken cancellationToken)
    {
        var executor = CqrsBuilder.Command<TeamDeleteCommand, bool>(services)
                                  .WithCommand(new(id))
                                  .Build();

        var result = await executor.ExecuteAsync(cancellationToken);
        return Ok(result);
    }
}