using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Consumables.DTOs;

namespace Tracer.Application.Features.Consumables.Queries;

public record GetAllConsumablesQuery : IRequest<List<ConsumableDto>>;

public sealed class GetAllConsumablesQueryHandler : IRequestHandler<GetAllConsumablesQuery, List<ConsumableDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetAllConsumablesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<ConsumableDto>> Handle(GetAllConsumablesQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUserService.CompanyId;
        var query = _context.Consumables.AsQueryable();

        if (companyId.HasValue)
            query = query.Where(c => c.CompanyId == companyId.Value);

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new ConsumableDto
            {
                Id = c.Id,
                Name = c.Name,
                CompanyId = c.CompanyId,
                TotalQuantity = c.TotalQuantity,
                PurchaseCost = c.PurchaseCost
            })
            .ToListAsync(cancellationToken);
    }
}
