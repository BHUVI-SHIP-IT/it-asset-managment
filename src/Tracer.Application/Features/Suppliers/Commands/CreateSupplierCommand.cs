using MediatR;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Entities;

namespace Tracer.Application.Features.Suppliers.Commands;

public record CreateSupplierCommand(string Name, Guid CompanyId) : IRequest<Guid>;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateSupplierCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var entity = new Supplier(Guid.NewGuid())
        {
            Name = request.Name
            , CompanyId = request.CompanyId
        };
        
        _context.Suppliers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
}
