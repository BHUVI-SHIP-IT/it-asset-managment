using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Dashboard.DTOs;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Domain.Aggregates.RequestAggregate;

namespace Tracer.Application.Features.Dashboard.Queries;

/// <summary>
/// User-scoped dashboard summary. Caller must pass the authenticated user's ID from JWT —
/// never a client-supplied target user id.
/// </summary>
public sealed record GetUserDashboardSummaryQuery(Guid UserId) : IRequest<UserDashboardSummaryDto>;

public sealed class GetUserDashboardSummaryQueryHandler
    : IRequestHandler<GetUserDashboardSummaryQuery, UserDashboardSummaryDto>
{
    private static readonly TimeSpan LicenseExpiringSoonWindow = TimeSpan.FromDays(30);
    private static readonly TimeSpan CheckoutReturnThreshold = TimeSpan.FromDays(90);
    private static readonly TimeSpan CheckoutReturnWarningWindow = TimeSpan.FromDays(14);

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUserDashboardSummaryQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<UserDashboardSummaryDto> Handle(
        GetUserDashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        // Defense in depth: never return another user's summary even if a buggy caller passes a wrong id.
        var authenticatedUserId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("No user context is available.");
        if (request.UserId != authenticatedUserId)
            throw new UnauthorizedAccessException("User summary is only available for the authenticated user.");

        var userId = authenticatedUserId;

        var assetCount = await _context.Assets.AsNoTracking()
            .CountAsync(a => a.AssignedUserId == userId && a.CompanyId == companyId, cancellationToken);

        var consumableCount = await _context.Consumables.AsNoTracking()
            .CountAsync(c => c.AssignedUserId == userId && c.CompanyId == companyId, cancellationToken);

        var componentCount = await _context.Components.AsNoTracking()
            .CountAsync(c => c.AssignedUserId == userId && c.CompanyId == companyId, cancellationToken);

        var accessoryCount = await _context.Accessories.AsNoTracking()
            .CountAsync(a => a.AssignedUserId == userId && a.CompanyId == companyId, cancellationToken);

        var licenseCount = await (
            from seat in _context.LicenseSeats.AsNoTracking()
            join license in _context.SoftwareLicenses.AsNoTracking()
                on seat.SoftwareLicenseId equals license.Id
            where seat.AssignedUserId == userId && license.CompanyId == companyId
            select seat.Id).CountAsync(cancellationToken);

        var requestRows = await _context.InventoryRequests.AsNoTracking()
            .Where(r => r.CompanyId == companyId && r.RequestedByUserId == userId)
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int CountFor(RequestStatus status) =>
            requestRows.FirstOrDefault(r => r.Status == status)?.Count ?? 0;

        var now = DateTime.UtcNow;
        var attention = new List<AttentionItemDto>();

        var licenseExpiringCutoff = now.Add(LicenseExpiringSoonWindow);
        var expiringLicenses = await (
            from seat in _context.LicenseSeats.AsNoTracking()
            join license in _context.SoftwareLicenses.AsNoTracking()
                on seat.SoftwareLicenseId equals license.Id
            where seat.AssignedUserId == userId
                  && license.CompanyId == companyId
                  && license.ExpirationDate != null
                  && license.ExpirationDate <= licenseExpiringCutoff
            orderby license.ExpirationDate
            select new { license.Name, license.ExpirationDate })
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var lic in expiringLicenses)
        {
            var expired = lic.ExpirationDate < now;
            attention.Add(new AttentionItemDto
            {
                Kind = expired ? "LicenseExpired" : "LicenseExpiring",
                Title = lic.Name,
                Detail = expired
                    ? $"License expired on {lic.ExpirationDate:yyyy-MM-dd}"
                    : $"License expires on {lic.ExpirationDate:yyyy-MM-dd}",
                DueAtUtc = lic.ExpirationDate
            });
        }

        // Assets use a 90-day checkout window (same as admin overdue); warn within 14 days of that due date.
        var returnDueCutoff = now.Subtract(CheckoutReturnThreshold).Add(CheckoutReturnWarningWindow);
        var assetsNeedingReturn = await _context.Assets.AsNoTracking()
            .Where(a => a.AssignedUserId == userId
                        && a.CompanyId == companyId
                        && a.Status == AssetStatus.Deployed
                        && a.CheckedOutAtUtc != null
                        && a.CheckedOutAtUtc <= returnDueCutoff)
            .OrderBy(a => a.CheckedOutAtUtc)
            .Select(a => new { a.Name, a.AssetTag, a.CheckedOutAtUtc })
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var asset in assetsNeedingReturn)
        {
            var dueAt = asset.CheckedOutAtUtc!.Value.Add(CheckoutReturnThreshold);
            var overdue = dueAt < now;
            attention.Add(new AttentionItemDto
            {
                Kind = overdue ? "ReturnOverdue" : "ReturnDueSoon",
                Title = string.IsNullOrWhiteSpace(asset.AssetTag) ? asset.Name : $"{asset.Name} ({asset.AssetTag})",
                Detail = overdue
                    ? $"Return was due on {dueAt:yyyy-MM-dd}"
                    : $"Return due on {dueAt:yyyy-MM-dd}",
                DueAtUtc = dueAt
            });
        }

        return new UserDashboardSummaryDto
        {
            AssignedCounts = new AssignedCountsDto
            {
                Assets = assetCount,
                Consumables = consumableCount,
                Components = componentCount,
                Licenses = licenseCount,
                Accessories = accessoryCount
            },
            RequestCounts = new RequestCountsDto
            {
                Pending = CountFor(RequestStatus.Pending),
                Approved = CountFor(RequestStatus.Approved),
                Rejected = CountFor(RequestStatus.Rejected)
            },
            AttentionItems = attention
                .OrderBy(a => a.DueAtUtc ?? DateTime.MaxValue)
                .ToList()
        };
    }
}
