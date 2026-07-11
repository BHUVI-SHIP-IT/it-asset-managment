using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Shared.Models;

namespace Tracer.Application.Features.Components;

public sealed record ComponentDto(int Id, string Name, Guid CompanyId, int TotalQuantity, decimal PurchaseCost);

public sealed record GetComponentsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortColumn = null,
    string? SortDirection = null) : IRequest<PagedList<ComponentDto>>;

public sealed class GetComponentsQueryHandler : IRequestHandler<GetComponentsQuery, PagedList<ComponentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetComponentsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedList<ComponentDto>> Handle(GetComponentsQuery request, CancellationToken cancellationToken)
    {
        var page = request.PageNumber < 1 ? 1 : request.PageNumber;
        var size = request.PageSize < 1 ? 10 : request.PageSize;
        var query = _context.Components.AsQueryable();

        if (_currentUser.CompanyId is Guid companyId)
            query = query.Where(x => x.CompanyId == companyId);

        query = (request.SortColumn?.ToLowerInvariant(), request.SortDirection?.ToLowerInvariant()) switch
        {
            ("totalquantity", "desc") => query.OrderByDescending(x => x.TotalQuantity),
            ("totalquantity", _) => query.OrderBy(x => x.TotalQuantity),
            ("purchasecost", "desc") => query.OrderByDescending(x => x.PurchaseCost),
            ("purchasecost", _) => query.OrderBy(x => x.PurchaseCost),
            ("name", "desc") => query.OrderByDescending(x => x.Name),
            _ => query.OrderBy(x => x.Name)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new ComponentDto(x.Id, x.Name, x.CompanyId, x.TotalQuantity, x.PurchaseCost))
            .ToListAsync(cancellationToken);

        return new PagedList<ComponentDto>(items, page, size, total);
    }
}

public sealed record CreateComponentCommand(string Name, int TotalQuantity, decimal PurchaseCost) : IRequest<int>;
public sealed record UpdateComponentCommand(int Id, string Name, int TotalQuantity, decimal PurchaseCost) : IRequest;
public sealed record DeleteComponentCommand(int Id) : IRequest;

public sealed class CreateComponentCommandHandler : IRequestHandler<CreateComponentCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateComponentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateComponentCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available.");

        var entity = Domain.Aggregates.InventoryAggregate.Component.Create(
            request.Name, companyId, request.TotalQuantity, request.PurchaseCost);

        _context.Components.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public sealed class UpdateComponentCommandHandler : IRequestHandler<UpdateComponentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateComponentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateComponentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Components.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Component {request.Id} was not found.");

        entity.Update(request.Name, request.TotalQuantity, request.PurchaseCost);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public sealed class DeleteComponentCommandHandler : IRequestHandler<DeleteComponentCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteComponentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteComponentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Components.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Component {request.Id} was not found.");

        entity.SoftDelete();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
