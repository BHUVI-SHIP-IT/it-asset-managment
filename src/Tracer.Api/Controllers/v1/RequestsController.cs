using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Requests.Commands;
using Tracer.Application.Features.Requests.Queries;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/requests")]
[Authorize]
[Produces("application/json")]
public sealed class RequestsController : ControllerBase
{
    private readonly ISender _sender;

    public RequestsController(ISender sender) => _sender = sender;

    [HttpPost]
    [Authorize(Policy = Permissions.Requests.Create)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRequestCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(new ProblemDetails { Detail = result.Error.Description });

        return CreatedAtAction(nameof(GetMine), new { }, result.Value);
    }

    [HttpGet("mine")]
    [Authorize(Policy = Permissions.Requests.ViewOwn)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var items = await _sender.Send(new GetMyRequestsQuery(), ct);
        return Ok(items);
    }

    [HttpGet]
    [Authorize(Policy = Permissions.Requests.ViewAll)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery] string? status, CancellationToken ct)
    {
        var items = await _sender.Send(new GetAllRequestsQuery(status), ct);
        return Ok(items);
    }

    [HttpGet("catalog")]
    [Authorize(Policy = Permissions.Requests.Create)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Catalog([FromQuery] string type, CancellationToken ct)
    {
        var items = await _sender.Send(new GetRequestCatalogQuery(type), ct);
        return Ok(items);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = Permissions.Requests.Approve)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ResolveNotesBody? body, CancellationToken ct)
    {
        var result = await _sender.Send(new ResolveRequestCommand(id, true, body?.Notes), ct);
        if (result.IsFailure)
        {
            if (result.Error.Type == Tracer.Shared.Results.ErrorType.NotFound)
                return NotFound(new ProblemDetails { Detail = result.Error.Description });
            return BadRequest(new ProblemDetails { Detail = result.Error.Description });
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = Permissions.Requests.Approve)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ResolveNotesBody? body, CancellationToken ct)
    {
        var result = await _sender.Send(new ResolveRequestCommand(id, false, body?.Notes), ct);
        if (result.IsFailure)
        {
            if (result.Error.Type == Tracer.Shared.Results.ErrorType.NotFound)
                return NotFound(new ProblemDetails { Detail = result.Error.Description });
            return BadRequest(new ProblemDetails { Detail = result.Error.Description });
        }

        return NoContent();
    }

    public sealed record ResolveNotesBody(string? Notes);
}
