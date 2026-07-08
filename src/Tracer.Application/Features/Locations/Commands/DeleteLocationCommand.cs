using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Locations.Commands;

public record DeleteLocationCommand(Guid Id) : IRequest<bool>;

public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteLocationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Locations.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        _context.Locations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
