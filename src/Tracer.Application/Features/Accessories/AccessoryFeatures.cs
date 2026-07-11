using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Shared.Models;

namespace Tracer.Application.Features.Accessories;

public sealed record AccessoryDto(int Id, string Name, Guid CompanyId, int TotalQuantity, decimal PurchaseCost);

public sealed record GetAccessoriesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortColumn = null,
    string? SortDirection = null) : IRequest<PagedList<AccessoryDto>>;

public sealed class GetAccessoriesQueryHandler : IRequestHandler<GetAccessoriesQuery, PagedList<AccessoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAccessoriesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedList<AccessoryDto>> Handle(GetAccessoriesQuery request, CancellationToken cancellationToken)
    {
        var page = request.PageNumber < 1 ? 1 : request.PageNumber;
        var size = request.PageSize < 1 ? 10 : request.PageSize;
        var query = _context.Accessories.AsQueryable();

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
            .Select(x => new AccessoryDto(x.Id, x.Name, x.CompanyId, x.TotalQuantity, x.PurchaseCost))
            .ToListAsync(cancellationToken);

        return new PagedList<AccessoryDto>(items, page, size, total);
    }
}

public sealed record CreateAccessoryCommand(string Name, int TotalQuantity, decimal PurchaseCost) : IRequest<int>;
public sealed record UpdateAccessoryCommand(int Id, string Name, int TotalQuantity, decimal PurchaseCost) : IRequest;
public sealed record DeleteAccessoryCommand(int Id) : IRequest;

public sealed class CreateAccessoryCommandHandler : IRequestHandler<CreateAccessoryCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateAccessoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateAccessoryCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available.");

        var entity = Domain.Aggregates.InventoryAggregate.Accessory.Create(
            request.Name, companyId, request.TotalQuantity, request.PurchaseCost);

        _context.Accessories.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public sealed class UpdateAccessoryCommandHandler : IRequestHandler<UpdateAccessoryCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAccessoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateAccessoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Accessories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Accessory {request.Id} was not found.");

        entity.Update(request.Name, request.TotalQuantity, request.PurchaseCost);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public sealed class DeleteAccessoryCommandHandler : IRequestHandler<DeleteAccessoryCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAccessoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteAccessoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Accessories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Accessory {request.Id} was not found.");

        entity.SoftDelete();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
