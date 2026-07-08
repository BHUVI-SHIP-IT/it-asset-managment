using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracer.Application.Features.Reports.Commands.GenerateFinancialReport;
using Tracer.Application.Features.Reports.Queries.DownloadReport;
using Tracer.Application.Features.Reports.Queries.GetReportStatus;

namespace Tracer.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("financial")]
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
    public async Task<IActionResult> GetReportStatus(Guid id)
    {
        var result = await _mediator.Send(new GetReportStatusQuery(id));
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("financial/{id}/download")]
    public async Task<IActionResult> DownloadReport(Guid id)
    {
        var result = await _mediator.Send(new DownloadReportQuery(id));
        if (result.IsFailure)
        {
            // Usually we'd check error code for NotFound vs Conflict
            if (result.Error.Code == "Report.NotFound") return NotFound(result.Error);
            return Conflict(result.Error);
        }

        return File(result.Value.FileContent, "text/csv", result.Value.ReportName);
    }
}
