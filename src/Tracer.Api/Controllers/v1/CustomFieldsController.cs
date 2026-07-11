using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Shared.Authorization;
using Tracer.Application.Features.CustomFields.Commands;
using Tracer.Application.Features.CustomFields.Queries;
using Tracer.Domain.Aggregates.CustomFieldAggregate;

namespace Tracer.Api.Controllers.v1;

/// <summary>
/// Custom fields API (M6, Doc 4 §3.20–3.21). Manages tenant-defined field definitions
/// and their values on specific entity instances (e.g. Assets).
/// </summary>
[ApiController]
[Route("api/v1/custom-fields")]
[Produces("application/json")]
[Authorize]
public sealed class CustomFieldsController : ControllerBase
{
    private readonly ISender _sender;

    public CustomFieldsController(ISender sender) => _sender = sender;

    /// <summary>Lists all custom field definitions for the current tenant.</summary>
    [HttpGet]
    [Authorize(Policy = Permissions.CustomFields.View)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllCustomFieldsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets all custom field values for a specific entity (e.g. an Asset ID).</summary>
    [HttpGet("values/{entityId:guid}")]
    [Authorize(Policy = Permissions.CustomFields.View)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEntity(Guid entityId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCustomFieldsByEntityQuery(entityId), cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new custom field definition for the current tenant.</summary>
    [HttpPost]
    [Authorize(Policy = Permissions.CustomFields.Manage)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomFieldRequest body, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _sender.Send(
                new CreateCustomFieldCommand(body.Name, body.FieldType, body.IsRequired, body.Options),
                cancellationToken);

            return CreatedAtAction(nameof(GetAll), new { }, id);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }

    /// <summary>Updates an existing custom field definition.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permissions.CustomFields.Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomFieldRequest body, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _sender.Send(
                new UpdateCustomFieldCommand(id, body.Name, body.FieldType, body.IsRequired, body.Options),
                cancellationToken);

            if (!success) return NotFound();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
    }

    /// <summary>Soft-deletes a custom field definition (Doc 4 §1.2).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Permissions.CustomFields.Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _sender.Send(new DeleteCustomFieldCommand(id), cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Sets (creates or updates) a custom field value for a specific entity instance.
    /// Returns the value record ID.
    /// </summary>
    [HttpPut("{id:guid}/values/{entityId:guid}")]
    [Authorize(Policy = Permissions.CustomFields.Manage)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetValue(Guid id, Guid entityId, [FromBody] SetCustomFieldValueRequest body, CancellationToken cancellationToken)
    {
        var resultId = await _sender.Send(new SetCustomFieldValueCommand(id, entityId, body.Value), cancellationToken);
        return Ok(resultId);
    }
}

// ── Request body records ─────────────────────────────────────────────────────

public record CreateCustomFieldRequest(
    string Name,
    CustomFieldType FieldType,
    bool IsRequired,
    string? Options = null);

public record UpdateCustomFieldRequest(
    string Name,
    CustomFieldType FieldType,
    bool IsRequired,
    string? Options = null);

public record SetCustomFieldValueRequest(string? Value);
