using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Notifications.DTOs;

namespace Tracer.Application.Features.Notifications.Queries;

public record GetNotificationByIdQuery(Guid Id) : IRequest<NotificationDto?>;

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, NotificationDto?>
{
    private readonly IApplicationDbContext _context;
    public GetNotificationByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<NotificationDto?> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .Where(n => n.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
