using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Assets.Commands.CheckinAsset;
using Tracer.Application.Features.Assets.Commands.CheckoutAsset;
using Tracer.Application.Features.Assets.Commands.CreateAsset;
using Tracer.Application.Features.Assets.Commands.DeleteAsset;
using Tracer.Application.Features.Assets.Commands.UpdateAsset;
using Tracer.Application.Features.Assets.Queries;
using Tracer.Domain.Aggregates.AssetAggregate;

namespace Tracer.Api.Controllers.v1;

/// <summary>
/// Asset lifecycle API (Doc 5 §3.1). Routes map to MediatR commands/queries via vertical slices.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class AssetsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public AssetsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    /// <summary>Get paginated list of assets.</summary>
    [HttpGet]
    [Authorize(Policy = "Assets.View")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? searchTerm,
        [FromQuery] AssetStatus? status,
        [FromQuery] int? statusLabelId,
        [FromQuery] Guid? locationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var companyId = _currentUserService.CompanyId ?? Guid.Empty;
        var query = new GetAllAssetsQuery(
            companyId, searchTerm, status, statusLabelId, locationId, page, pageSize, sortBy, sortDescending);
        
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Register a new asset (Doc 8 §2.1).</summary>
    [HttpPost]
    [Authorize(Policy = "Assets.Create")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssetCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new ProblemDetails { Detail = result.Error.Description });

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>Get asset by ID.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Assets.View")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAssetByIdQuery(id), cancellationToken);
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>Update asset descriptive fields.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Assets.Edit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAssetCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Id != id)
            return BadRequest(new ProblemDetails { Detail = "Route ID does not match body." });

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new ProblemDetails { Detail = result.Error.Description });

        return NoContent();
    }

    /// <summary>Soft-delete an asset (Doc 4 §1.2).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Assets.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteAssetCommand(id), cancellationToken);

        if (result.IsFailure)
            return NotFound(new ProblemDetails { Detail = result.Error.Description });

        return NoContent();
    }

    /// <summary>Check out an asset to a user (Doc 3 §4.2).</summary>
    [HttpPost("{id:guid}/checkout")]
    [Authorize(Policy = "Assets.CheckOut")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Checkout(
        Guid id,
        [FromBody] CheckoutAssetCommand command,
        CancellationToken cancellationToken)
    {
        if (command.AssetId != id)
            return BadRequest(new ProblemDetails { Detail = "Route ID does not match body." });

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return UnprocessableEntity(new ProblemDetails { Detail = result.Error.Description });
        return NoContent();
    }

    /// <summary>Check in an asset (return to inventory) (Doc 3 §4.2).</summary>
    [HttpPost("{id:guid}/checkin")]
    [Authorize(Policy = "Assets.CheckIn")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Checkin(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CheckinAssetCommand(id), cancellationToken);

        if (result.IsFailure)
            return UnprocessableEntity(new ProblemDetails { Detail = result.Error.Description });

        return NoContent();
}
}
