using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.StatusLabels.DTOs;

namespace Tracer.Application.Features.StatusLabels.Queries;

public record GetStatusLabelByIdQuery(int Id) : IRequest<StatusLabelDto?>;

public class GetStatusLabelByIdQueryHandler : IRequestHandler<GetStatusLabelByIdQuery, StatusLabelDto?>
{
    private readonly IApplicationDbContext _context;
    public GetStatusLabelByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<StatusLabelDto?> Handle(GetStatusLabelByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.StatusLabels.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return null;
        return new StatusLabelDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsDeployable = entity.IsDeployable,
            IsPending = entity.IsPending,
            IsArchived = entity.IsArchived
        };
    }
}
