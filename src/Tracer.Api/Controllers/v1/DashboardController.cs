using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Dashboard.DTOs;
using Tracer.Application.Features.Dashboard.Queries;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
[Produces("application/json")]
public sealed class DashboardController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Aggregate asset metrics for the current tenant dashboard.</summary>
    [HttpGet("metrics")]
    [Authorize(Policy = Permissions.Assets.View)]
    [ProducesResponseType(typeof(DashboardMetricsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardMetricsDto>> GetMetrics(CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId ?? Guid.Empty;
        var result = await _sender.Send(new GetDashboardMetricsQuery(companyId), cancellationToken);
        return Ok(result);
    }
}
