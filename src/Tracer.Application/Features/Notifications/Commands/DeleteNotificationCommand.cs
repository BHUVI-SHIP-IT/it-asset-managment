using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Notifications.Commands;

public record DeleteNotificationCommand(Guid Id) : IRequest<bool>;

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteNotificationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);
        if (entity == null) return false;

        // Interceptor converts Remove to soft-delete (Doc 4 §1.2).
        _context.Notifications.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
