using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Reports.Queries.DownloadReport;

public sealed class DownloadReportQueryHandler : IRequestHandler<DownloadReportQuery, Result<ReportDownloadDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DownloadReportQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ReportDownloadDto>> Handle(DownloadReportQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        var report = await _context.ReportExports.FindAsync(new object[] { request.ReportId }, cancellationToken);

        if (report == null || report.CompanyId != companyId)
        {
            return Result.Failure<ReportDownloadDto>(Error.NotFound("Report.NotFound", "The requested report was not found."));
        }

        if (report.Status != ReportStatus.Completed || report.FileContent == null)
        {
            return Result.Failure<ReportDownloadDto>(Error.Conflict("Report.NotReady", "The report is not ready for download."));
        }

        return Result.Success(new ReportDownloadDto($"{report.ReportName}.csv", report.FileContent));
    }
}
