using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tracer.Application.Features.Reports.Commands.GenerateFinancialReport;
using Tracer.Application.Features.Reports.Queries.DownloadReport;
using Tracer.Application.Features.Reports.Queries.GetAllReports;
using Tracer.Application.Features.Reports.Queries.GetReportStatus;
using Tracer.Shared.Authorization;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/reports")]
[Authorize]
[EnableRateLimiting(RateLimitPolicies.Reports)]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.Reports.View)]
    public async Task<ActionResult<List<ReportListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetAllReportsQuery(), cancellationToken));
    }

    /// <summary>Frontend-compatible create endpoint (maps to financial report generation).</summary>
    [HttpPost]
    [Authorize(Policy = Permissions.Reports.Export)]
    public async Task<IActionResult> Create([FromBody] CreateReportRequest request)
    {
        var reportName = string.IsNullOrWhiteSpace(request.ReportType)
            ? "Financial Report"
            : request.ReportType;

        var result = await _mediator.Send(new GenerateFinancialReportCommand(reportName));
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Accepted(result.Value);
    }

    [HttpPost("financial")]
    [Authorize(Policy = Permissions.Reports.Export)]
    public async Task<IActionResult> GenerateFinancialReport([FromBody] GenerateFinancialReportCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Accepted(new { ReportId = result.Value });
    }

    [HttpGet("financial/{id}")]
    [Authorize(Policy = Permissions.Reports.View)]
    public async Task<IActionResult> GetReportStatus(Guid id)
    {
        var result = await _mediator.Send(new GetReportStatusQuery(id));
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/download")]
    [HttpGet("financial/{id:guid}/download")]
    [Authorize(Policy = Permissions.Reports.Export)]
    public async Task<IActionResult> DownloadReport(Guid id)
    {
        var result = await _mediator.Send(new DownloadReportQuery(id));
        if (result.IsFailure)
        {
            if (result.Error.Code == "Report.NotFound") return NotFound(result.Error);
            return Conflict(result.Error);
        }

        return File(result.Value.FileContent, "text/csv", result.Value.ReportName);
    }
}

public sealed record CreateReportRequest(string? ReportType);
