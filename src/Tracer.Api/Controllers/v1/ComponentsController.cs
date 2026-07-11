using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Components;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/components")]
[Authorize]
public sealed class ComponentsController : ControllerBase
{
    private readonly ISender _sender;

    public ComponentsController(ISender sender) => _sender = sender;

    [HttpGet]
    [Authorize(Policy = Permissions.Components.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortColumn = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetComponentsQuery(pageNumber, pageSize, sortColumn, sortDirection),
            cancellationToken);

        return Ok(new
        {
            items = result.Items,
            totalCount = result.TotalCount,
            pageNumber = result.Page,
            pageSize = result.PageSize
        });
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Components.Create)]
    public async Task<ActionResult<int>> Create([FromBody] CreateComponentCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permissions.Components.Update)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateComponentCommand body, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateComponentCommand(id, body.Name, body.TotalQuantity, body.PurchaseCost), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.Components.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteComponentCommand(id), cancellationToken);
        return NoContent();
    }
}
