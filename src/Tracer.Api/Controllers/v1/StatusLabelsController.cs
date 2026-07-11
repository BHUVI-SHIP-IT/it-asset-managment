using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Shared.Authorization;
using Tracer.Application.Features.StatusLabels.Commands;
using Tracer.Application.Features.StatusLabels.Queries;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/status-labels")]
[Authorize(Policy = Permissions.StatusLabels.View)]
public class StatusLabelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatusLabelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllStatusLabelsQuery());
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 10 : pageSize;
        var items = result.Skip((page - 1) * size).Take(size).ToList();
        return Ok(new
        {
            items,
            totalCount = result.Count,
            pageNumber = page,
            pageSize = size
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetStatusLabelByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = Permissions.StatusLabels.Create)]
    public async Task<IActionResult> Create(CreateStatusLabelCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.StatusLabels.Update)]
    public async Task<IActionResult> Update(int id, UpdateStatusLabelCommand command)
    {
        if (id != command.Id) return BadRequest();
        var success = await _mediator.Send(command);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.StatusLabels.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeleteStatusLabelCommand(id));
        return success ? NoContent() : NotFound();
    }
}
