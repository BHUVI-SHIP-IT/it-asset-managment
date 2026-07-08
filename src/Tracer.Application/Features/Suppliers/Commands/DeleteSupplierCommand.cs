using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Suppliers.Commands;

public record DeleteSupplierCommand(Guid Id) : IRequest<bool>;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteSupplierCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        _context.Suppliers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
