using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Tracer.Application.Features.Notifications.Commands;
using Tracer.Application.Features.Notifications.Queries;

namespace Tracer.Api.Controllers.v1;

/// <summary>
/// Notification center API (M6, Doc 5 §3.21). Provides paginated access to the tenant's
/// notification feed and allows marking notifications as read or deleting them.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender) => _sender = sender;

    /// <summary>Gets a paginated, newest-first list of notifications for the current tenant.</summary>
    /// <param name="page">1-based page number (default: 1).</param>
    /// <param name="pageSize">Items per page, max 200 (default: 50).</param>
    /// <param name="unreadOnly">When true, only returns unread notifications.</param>
    [HttpGet]
    [Authorize(Policy = "Notifications.View")]
    [OutputCache(PolicyName = "UserScoped30s")]
    [EnableRateLimiting("ReadPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool unreadOnly = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetAllNotificationsQuery(page, pageSize, unreadOnly), cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets a single notification by ID.</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Notifications.View")]
    [EnableRateLimiting("ReadPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetNotificationByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>Marks a notification as read.</summary>
    [HttpPost("{id:guid}/read")]
    [Authorize(Policy = "Notifications.View")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        var success = await _sender.Send(new MarkNotificationReadCommand(id), cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>Soft-deletes a notification (Doc 4 §1.2).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Notifications.Delete")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _sender.Send(new DeleteNotificationCommand(id), cancellationToken);
        if (!success) return NotFound();
        return NoContent();
    }
}
