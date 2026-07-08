using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Locations.DTOs;

namespace Tracer.Application.Features.Locations.Queries;

public record GetLocationByIdQuery(Guid Id) : IRequest<LocationDto?>;

public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, LocationDto?>
{
    private readonly IApplicationDbContext _context;
    public GetLocationByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<LocationDto?> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Locations.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return null;
        return new LocationDto { Id = entity.Id, Name = entity.Name };
    }
}
