using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Accessories;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/accessories")]
[Authorize]
public sealed class AccessoriesController : ControllerBase
{
    private readonly ISender _sender;

    public AccessoriesController(ISender sender) => _sender = sender;

    [HttpGet]
    [Authorize(Policy = Permissions.Accessories.View)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortColumn = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetAccessoriesQuery(pageNumber, pageSize, sortColumn, sortDirection),
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
    [Authorize(Policy = Permissions.Accessories.Create)]
    public async Task<ActionResult<int>> Create([FromBody] CreateAccessoryCommand command, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Permissions.Accessories.Update)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateAccessoryCommand body, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateAccessoryCommand(id, body.Name, body.TotalQuantity, body.PurchaseCost), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Permissions.Accessories.Delete)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteAccessoryCommand(id), cancellationToken);
        return NoContent();
    }
}
