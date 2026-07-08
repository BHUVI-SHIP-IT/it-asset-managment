using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Suppliers.DTOs;

namespace Tracer.Application.Features.Suppliers.Queries;

public record GetSupplierByIdQuery(Guid Id) : IRequest<SupplierDto?>;

public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto?>
{
    private readonly IApplicationDbContext _context;
    public GetSupplierByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<SupplierDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return null;
        return new SupplierDto { Id = entity.Id, Name = entity.Name };
    }
}
