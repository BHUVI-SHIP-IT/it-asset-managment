using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Notifications.Commands;

public record MarkNotificationReadCommand(Guid Id) : IRequest<bool>;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public MarkNotificationReadCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);
        if (entity == null) return false;

        entity.MarkRead();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
