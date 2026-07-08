using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Locations.DTOs;

namespace Tracer.Application.Features.Locations.Queries;

public record GetAllLocationsQuery : IRequest<List<LocationDto>>;

public class GetAllLocationsQueryHandler : IRequestHandler<GetAllLocationsQuery, List<LocationDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAllLocationsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<LocationDto>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .Select(x => new LocationDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }
}
