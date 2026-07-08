using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Notifications.DTOs;

namespace Tracer.Application.Features.Notifications.Queries;

/// <summary>
/// Paginated notification-center feed, newest first. Tenant-scoped via the current user's CompanyId
/// (RLS per Doc 5 §3.21). <paramref name="UnreadOnly"/> filters to undelivered/unread items.
/// </summary>
public record GetAllNotificationsQuery(int Page = 1, int PageSize = 50, bool UnreadOnly = false)
    : IRequest<List<NotificationDto>>;

public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, List<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllNotificationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<NotificationDto>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

        var companyId = _currentUser.CompanyId;

        var query = _context.Notifications.AsQueryable();

        // Tenant isolation: only surface this company's notifications (Doc 5 §3.21).
        if (companyId.HasValue)
            query = query.Where(n => n.CompanyId == companyId);

        if (request.UnreadOnly)
            query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Name,
                Body = n.Body,
                Severity = n.Severity.ToString(),
                Channel = n.Channel,
                Status = n.Status.ToString(),
                Recipient = n.Recipient,
                FailureReason = n.FailureReason,
                IsRead = n.IsRead,
                SentAtUtc = n.SentAtUtc,
                CreatedAtUtc = n.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
