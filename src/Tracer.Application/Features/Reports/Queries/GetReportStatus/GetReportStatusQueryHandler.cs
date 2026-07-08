using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Reports.Queries.GetReportStatus;

public sealed class GetReportStatusQueryHandler : IRequestHandler<GetReportStatusQuery, Result<ReportStatusDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetReportStatusQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ReportStatusDto>> Handle(GetReportStatusQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        var report = await _context.ReportExports.FindAsync(new object[] { request.ReportId }, cancellationToken);

        if (report == null || report.CompanyId != companyId)
        {
            return Result.Failure<ReportStatusDto>(Error.NotFound("Report.NotFound", "The requested report was not found."));
        }

        return Result.Success(new ReportStatusDto(
            report.Id,
            report.ReportName,
            report.Status,
            report.CreatedAtUtc,
            report.CompletedAtUtc));
    }
}
