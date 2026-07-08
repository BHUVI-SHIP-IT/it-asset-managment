using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.StatusLabels.DTOs;

namespace Tracer.Application.Features.StatusLabels.Queries;

public record GetAllStatusLabelsQuery : IRequest<List<StatusLabelDto>>;

public class GetAllStatusLabelsQueryHandler : IRequestHandler<GetAllStatusLabelsQuery, List<StatusLabelDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAllStatusLabelsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<StatusLabelDto>> Handle(GetAllStatusLabelsQuery request, CancellationToken cancellationToken)
    {
        return await _context.StatusLabels
            .Select(x => new StatusLabelDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }
}
