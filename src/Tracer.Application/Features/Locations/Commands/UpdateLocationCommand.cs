using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Locations.Commands;

public record UpdateLocationCommand(Guid Id, string Name) : IRequest<bool>;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public UpdateLocationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Locations.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
