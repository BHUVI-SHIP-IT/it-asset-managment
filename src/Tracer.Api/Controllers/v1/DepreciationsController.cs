using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Depreciations.Commands.CreateDepreciation;
using Tracer.Application.Features.Depreciations.Queries.GetAllDepreciations;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/depreciation")]
[Authorize]
public class DepreciationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepreciationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.Depreciation.View)]
    public async Task<ActionResult<List<DepreciationListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetAllDepreciationsQuery(), cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Depreciation.Create)]
    public async Task<IActionResult> Create([FromBody] CreateDepreciationCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { Id = result.Value });
    }
}
