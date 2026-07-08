using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Suppliers.DTOs;

namespace Tracer.Application.Features.Suppliers.Queries;

public record GetAllSuppliersQuery : IRequest<List<SupplierDto>>;

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, List<SupplierDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAllSuppliersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<SupplierDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Suppliers
            .Select(x => new SupplierDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }
}
