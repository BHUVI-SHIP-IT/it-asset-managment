using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Manufacturers.Commands;

public record DeleteManufacturerCommand(Guid Id) : IRequest<bool>;

public class DeleteManufacturerCommandHandler : IRequestHandler<DeleteManufacturerCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteManufacturerCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteManufacturerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Manufacturers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        _context.Manufacturers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
