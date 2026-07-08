using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Settings.Commands;
using Tracer.Application.Features.Settings.Queries;

namespace Tracer.Api.Controllers.v1;

/// <summary>
/// Tenant settings API (M6, Doc 5 §settings). Manages per-tenant key/value configuration
/// used to enable/configure notification channels (e.g. Slack webhook URL, SMTP host).
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class SettingsController : ControllerBase
{
    private readonly ISender _sender;

    public SettingsController(ISender sender) => _sender = sender;

    /// <summary>Gets all tenant settings for the current company.</summary>
    [HttpGet]
    [Authorize(Policy = "Settings.View")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllSettingsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates or updates a setting by key (upsert). Returns the setting ID.</summary>
    [HttpPut("{key}")]
    [Authorize(Policy = "Settings.Manage")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upsert(string key, [FromBody] UpsertSettingRequest body, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
            return BadRequest(new ProblemDetails { Detail = "Setting key is required." });

        var id = await _sender.Send(new UpsertSettingCommand(key, body.Value), cancellationToken);
        return Ok(id);
    }

    /// <summary>Deletes a setting by key (soft-delete). Returns 404 when the key does not exist.</summary>
    [HttpDelete("{key}")]
    [Authorize(Policy = "Settings.Manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string key, CancellationToken cancellationToken)
    {
        var success = await _sender.Send(new DeleteSettingCommand(key), cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
}

/// <summary>Request body for the upsert endpoint.</summary>
public record UpsertSettingRequest(string? Value);
