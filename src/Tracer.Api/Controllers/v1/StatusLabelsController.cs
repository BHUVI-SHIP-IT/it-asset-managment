using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.StatusLabels.Commands;
using Tracer.Application.Features.StatusLabels.Queries;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "StatusLabels.View")]
public class StatusLabelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatusLabelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllStatusLabelsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetStatusLabelByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [Authorize(Policy = "StatusLabels.Create")]
    public async Task<IActionResult> Create(CreateStatusLabelCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "StatusLabels.Update")]
    public async Task<IActionResult> Update(int id, UpdateStatusLabelCommand command)
    {
        if (id != command.Id) return BadRequest();
        var success = await _mediator.Send(command);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "StatusLabels.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _mediator.Send(new DeleteStatusLabelCommand(id));
        return success ? NoContent() : NotFound();
    }
}
