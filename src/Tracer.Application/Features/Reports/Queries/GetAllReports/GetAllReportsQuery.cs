using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Reports.Queries.GetAllReports;

public sealed record ReportListItemDto(
    Guid Id,
    string ReportType,
    DateTime RequestedAt,
    DateTime? CompletedAt,
    string Status,
    string? S3Url);

public sealed record GetAllReportsQuery : IRequest<List<ReportListItemDto>>;

public sealed class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, List<ReportListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllReportsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ReportListItemDto>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ReportExports.AsQueryable();
        if (_currentUser.CompanyId is Guid companyId)
            query = query.Where(x => x.CompanyId == companyId);

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ReportListItemDto(
                x.Id,
                x.ReportName,
                x.CreatedAtUtc,
                x.CompletedAtUtc,
                x.Status.ToString(),
                null))
            .ToListAsync(cancellationToken);
    }
}
