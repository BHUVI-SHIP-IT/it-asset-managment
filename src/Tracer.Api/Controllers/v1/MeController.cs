using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Users.Queries;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

/// <summary>
/// Current-user scoped endpoints (no Users.View required).
/// </summary>
[ApiController]
[Route("api/v1/me")]
[Authorize]
[Produces("application/json")]
public sealed class MeController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public MeController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    [HttpGet("assigned-items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyAssignedItems(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Unauthorized();

        var result = await _sender.Send(new GetUserAssignedItemsQuery(userId.Value), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
