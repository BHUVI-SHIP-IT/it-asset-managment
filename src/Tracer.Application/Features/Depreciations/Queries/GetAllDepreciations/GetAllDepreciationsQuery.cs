using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Application.Features.Depreciations.Queries.GetAllDepreciations;

public sealed record DepreciationListItemDto(
    Guid Id,
    string Name,
    int Months,
    decimal MinimumValue,
    Guid CompanyId);

public sealed record GetAllDepreciationsQuery : IRequest<List<DepreciationListItemDto>>;

public sealed class GetAllDepreciationsQueryHandler
    : IRequestHandler<GetAllDepreciationsQuery, List<DepreciationListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllDepreciationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<DepreciationListItemDto>> Handle(
        GetAllDepreciationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Depreciations.AsQueryable();
        if (_currentUser.CompanyId is Guid companyId)
            query = query.Where(x => x.CompanyId == companyId);

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new DepreciationListItemDto(x.Id, x.Name, x.Months, x.MinimumValue, x.CompanyId))
            .ToListAsync(cancellationToken);
    }
}
