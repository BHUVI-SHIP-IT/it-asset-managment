using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Dashboard.DTOs;
using Tracer.Domain.Aggregates.AssetAggregate;

namespace Tracer.Application.Features.Dashboard.Queries;

public record GetDashboardMetricsQuery(Guid CompanyId) : IRequest<DashboardMetricsDto>;

public sealed class GetDashboardMetricsQueryHandler : IRequestHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
{
    private static readonly TimeSpan OverdueCheckoutThreshold = TimeSpan.FromDays(90);

    private readonly IApplicationDbContext _context;

    public GetDashboardMetricsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<DashboardMetricsDto> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        var assets = _context.Assets.Where(a => a.CompanyId == request.CompanyId);

        var totalAssets = await assets.CountAsync(cancellationToken);
        var activeAssets = await assets.CountAsync(a => a.Status == AssetStatus.Deployed, cancellationToken);
        var pendingCheckouts = await assets.CountAsync(a => a.Status == AssetStatus.Pending, cancellationToken);

        var overdueCutoff = DateTime.UtcNow.Subtract(OverdueCheckoutThreshold);
        var overdueCheckins = await assets.CountAsync(
            a => a.Status == AssetStatus.Deployed
                 && a.CheckedOutAtUtc != null
                 && a.CheckedOutAtUtc < overdueCutoff,
            cancellationToken);

        return new DashboardMetricsDto
        {
            TotalAssets = totalAssets,
            ActiveAssets = activeAssets,
            PendingCheckouts = pendingCheckouts,
            OverdueCheckins = overdueCheckins
        };
    }
}
