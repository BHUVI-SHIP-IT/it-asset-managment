using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Manufacturers.DTOs;

namespace Tracer.Application.Features.Manufacturers.Queries;

public record GetAllManufacturersQuery : IRequest<List<ManufacturerDto>>;

public class GetAllManufacturersQueryHandler : IRequestHandler<GetAllManufacturersQuery, List<ManufacturerDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAllManufacturersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ManufacturerDto>> Handle(GetAllManufacturersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Manufacturers
            .Select(x => new ManufacturerDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }
}
