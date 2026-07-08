using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Manufacturers.Commands;

public record UpdateManufacturerCommand(Guid Id, string Name) : IRequest<bool>;

public class UpdateManufacturerCommandHandler : IRequestHandler<UpdateManufacturerCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public UpdateManufacturerCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateManufacturerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Manufacturers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
