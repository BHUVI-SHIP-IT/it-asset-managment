using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Manufacturers.DTOs;

namespace Tracer.Application.Features.Manufacturers.Queries;

public record GetManufacturerByIdQuery(Guid Id) : IRequest<ManufacturerDto?>;

public class GetManufacturerByIdQueryHandler : IRequestHandler<GetManufacturerByIdQuery, ManufacturerDto?>
{
    private readonly IApplicationDbContext _context;
    public GetManufacturerByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ManufacturerDto?> Handle(GetManufacturerByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Manufacturers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return null;
        return new ManufacturerDto { Id = entity.Id, Name = entity.Name };
    }
}
