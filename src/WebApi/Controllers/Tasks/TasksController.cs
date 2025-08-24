using Application.UseCases.TaskItems.Create;
using Application.UseCases.TaskItems.Delete;
using Application.UseCases.TaskItems.Get;
using Application.UseCases.TaskItems.GetById;
using Application.UseCases.TaskItems.Update;

using Domain.Entities.TaskItems;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Shared.Enums;
using Shared.Models.TaskItems;
using Shared.Pagination;

using Softoverse.CqrsKit.Builders;
using Softoverse.CqrsKit.Models;

namespace WebApi.Controllers.Tasks;

[Route("api/[controller]")]
[ApiController]
public class TasksController(IServiceProvider services) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public async Task<ActionResult<Result<PagedData<TaskItemViewModel>>>> Get([FromQuery] TaskItemGetQuery query,
                                                                              [FromQuery] Pageable pageable,
                                                                              [FromQuery] Sortable sortable,
                                                                              CancellationToken ct = default)
    {
        query = query ?? new();
        query.Pageable = pageable;
        query.Sortable = sortable;

        var executor = CqrsBuilder.Query<TaskItemGetQuery, PagedData<TaskItemViewModel>>(services)
                                  .WithQuery(query)
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    public async Task<ActionResult<Result<TaskItemViewModel>>> GetById(long id,
                                                                       CancellationToken ct = default)
    {
        var executor = CqrsBuilder.Query<TaskItemGetByIdQuery, TaskItemViewModel>(services)
                                  .WithQuery(new TaskItemGetByIdQuery
                                  {
                                      Id = id
                                  })
                                  .Build();

        var result = await executor.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<Task>> Post([FromBody] TaskItemViewModel payload,
                                               CancellationToken cancellationToken)
    {
        var executor = CqrsBuilder.Command<TaskItemCreateCommand, TaskItemViewModel>(services)
                                  .WithCommand(new(payload))
                                  .Build();

        var result = await executor.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<Task>> Put([FromRoute] long id,
                                              [FromBody] TaskItemViewModel payload,
                                              CancellationToken cancellationToken)
    {
        var executor = CqrsBuilder.Command<TaskItemUpdateCommand, TaskItemViewModel>(services)
                                  .WithCommand(new(payload)
                                  {
                                      Id = id
                                  })
                                  .Build();

        var result = await executor.ExecuteAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:long}/status")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<Task>> UpdateStatus([FromRoute] long id,
                                                       [FromQuery] TaskItemStatus status,
                                                       CancellationToken cancellationToken)
    {
        var executor = CqrsBuilder.Command<TaskItemUpdateStatusCommand, bool>(services)
                                  .WithCommand(new(status)
                                  {
                                      Id = id
                                  })
                                  .Build();

        var result = await executor.ExecuteAsync(cancellationToken);
        return Ok(result);
    }


    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<bool>> Delete([FromRoute] long id,
                                                 CancellationToken cancellationToken)
    {
        var executor = CqrsBuilder.Command<TaskItemDeleteCommand, bool>(services)
                                  .WithCommand(new(id))
                                  .Build();

        var result = await executor.ExecuteAsync(cancellationToken);
        return Ok(result);
    }
}