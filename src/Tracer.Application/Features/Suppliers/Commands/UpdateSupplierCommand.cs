using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Suppliers.Commands;

public record UpdateSupplierCommand(Guid Id, string Name) : IRequest<bool>;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public UpdateSupplierCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        
        entity.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
