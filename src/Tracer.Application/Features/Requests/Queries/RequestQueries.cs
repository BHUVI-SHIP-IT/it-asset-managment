using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Requests.DTOs;
using Tracer.Domain.Aggregates.RequestAggregate;

namespace Tracer.Application.Features.Requests.Queries;

public sealed record GetMyRequestsQuery : IRequest<IReadOnlyList<RequestDto>>;

public sealed class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, IReadOnlyList<RequestDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyRequestsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<RequestDto>> Handle(GetMyRequestsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context.");
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("No user context.");

        var rows = await _db.InventoryRequests.AsNoTracking()
            .Where(r => r.CompanyId == companyId && r.RequestedByUserId == userId)
            .OrderByDescending(r => r.RequestedAtUtc)
            .ToListAsync(cancellationToken);

        return await RequestDtoMapper.MapAsync(_db, rows, cancellationToken);
    }
}

public sealed record GetAllRequestsQuery(string? Status = null) : IRequest<IReadOnlyList<RequestDto>>;

public sealed class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, IReadOnlyList<RequestDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAllRequestsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<RequestDto>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context.");

        var query = _db.InventoryRequests.AsNoTracking()
            .Where(r => r.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(request.Status)
            && Enum.TryParse<RequestStatus>(request.Status, ignoreCase: true, out var status))
        {
            query = query.Where(r => r.Status == status);
        }

        var rows = await query
            .OrderByDescending(r => r.RequestedAtUtc)
            .ToListAsync(cancellationToken);

        return await RequestDtoMapper.MapAsync(_db, rows, cancellationToken);
    }
}

public sealed record GetRequestCatalogQuery(string Type) : IRequest<IReadOnlyList<RequestCatalogItemDto>>;

public sealed class GetRequestCatalogQueryHandler
    : IRequestHandler<GetRequestCatalogQuery, IReadOnlyList<RequestCatalogItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetRequestCatalogQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<RequestCatalogItemDto>> Handle(
        GetRequestCatalogQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context.");

        if (!Enum.TryParse<RequestType>(request.Type, ignoreCase: true, out var type))
            return [];

        return type switch
        {
            RequestType.Asset => await _db.Assets.AsNoTracking()
                .Where(a => a.CompanyId == companyId && a.AssignedUserId == null)
                .OrderBy(a => a.Name)
                .Select(a => new RequestCatalogItemDto(a.Id.ToString(), a.Name, a.AssetTag))
                .ToListAsync(cancellationToken),
            RequestType.Consumable => await _db.Consumables.AsNoTracking()
                .Where(c => c.CompanyId == companyId && c.TotalQuantity > 0)
                .OrderBy(c => c.Name)
                .Select(c => new RequestCatalogItemDto(c.Id.ToString(), c.Name, c.TotalQuantity.ToString()))
                .ToListAsync(cancellationToken),
            RequestType.Component => await _db.Components.AsNoTracking()
                .Where(c => c.CompanyId == companyId && c.TotalQuantity > 0)
                .OrderBy(c => c.Name)
                .Select(c => new RequestCatalogItemDto(c.Id.ToString(), c.Name, c.TotalQuantity.ToString()))
                .ToListAsync(cancellationToken),
            RequestType.Accessory => await _db.Accessories.AsNoTracking()
                .Where(a => a.CompanyId == companyId && a.TotalQuantity > 0)
                .OrderBy(a => a.Name)
                .Select(a => new RequestCatalogItemDto(a.Id.ToString(), a.Name, a.TotalQuantity.ToString()))
                .ToListAsync(cancellationToken),
            RequestType.LicenseRenewal => await _db.SoftwareLicenses.AsNoTracking()
                .Where(l => l.CompanyId == companyId)
                .OrderBy(l => l.Name)
                .Select(l => new RequestCatalogItemDto(
                    l.Id.ToString(),
                    l.Name,
                    l.ExpirationDate.HasValue ? l.ExpirationDate.Value.ToString("u") : null))
                .ToListAsync(cancellationToken),
            _ => []
        };
    }
}

internal static class RequestDtoMapper
{
    public static async Task<IReadOnlyList<RequestDto>> MapAsync(
        IApplicationDbContext db,
        IReadOnlyList<InventoryRequest> rows,
        CancellationToken ct)
    {
        if (rows.Count == 0)
            return [];

        var userIds = rows.Select(r => r.RequestedByUserId)
            .Concat(rows.Where(r => r.ResolvedByUserId.HasValue).Select(r => r.ResolvedByUserId!.Value))
            .Distinct()
            .ToList();

        var names = await db.Users.AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, ct);

        var result = new List<RequestDto>(rows.Count);
        foreach (var r in rows)
        {
            result.Add(new RequestDto
            {
                Id = r.Id,
                Type = r.Type.ToString(),
                RequestedByUserId = r.RequestedByUserId,
                RequestedByName = names.GetValueOrDefault(r.RequestedByUserId, string.Empty),
                ItemId = r.ItemId,
                ItemName = await ResolveItemNameAsync(db, r, ct),
                Quantity = r.Quantity,
                Status = r.Status.ToString(),
                RequestedAtUtc = r.RequestedAtUtc,
                ResolvedByUserId = r.ResolvedByUserId,
                ResolvedByName = r.ResolvedByUserId is Guid rid
                    ? names.GetValueOrDefault(rid, string.Empty)
                    : null,
                ResolvedAtUtc = r.ResolvedAtUtc,
                Notes = r.Notes,
                ResolutionNotes = r.ResolutionNotes
            });
        }

        return result;
    }

    private static async Task<string?> ResolveItemNameAsync(
        IApplicationDbContext db, InventoryRequest r, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(r.ItemId))
            return null;

        if (r.Type == RequestType.Return)
            return await ResolveReturnItemNameAsync(db, r.ItemId, ct);

        return r.Type switch
        {
            RequestType.Asset when Guid.TryParse(r.ItemId, out var assetId) =>
                await db.Assets.AsNoTracking().Where(a => a.Id == assetId).Select(a => a.Name).FirstOrDefaultAsync(ct),
            RequestType.Consumable when int.TryParse(r.ItemId, out var cId) =>
                await db.Consumables.AsNoTracking().Where(c => c.Id == cId).Select(c => c.Name).FirstOrDefaultAsync(ct),
            RequestType.Component when int.TryParse(r.ItemId, out var pId) =>
                await db.Components.AsNoTracking().Where(c => c.Id == pId).Select(c => c.Name).FirstOrDefaultAsync(ct),
            RequestType.Accessory when int.TryParse(r.ItemId, out var aId) =>
                await db.Accessories.AsNoTracking().Where(a => a.Id == aId).Select(a => a.Name).FirstOrDefaultAsync(ct),
            RequestType.LicenseRenewal when Guid.TryParse(r.ItemId, out var lId) =>
                await db.SoftwareLicenses.AsNoTracking().Where(l => l.Id == lId).Select(l => l.Name).FirstOrDefaultAsync(ct),
            _ => null
        };
    }

    private static async Task<string?> ResolveReturnItemNameAsync(
        IApplicationDbContext db, string itemId, CancellationToken ct)
    {
        var sep = itemId.IndexOf(':');
        if (sep <= 0 || sep >= itemId.Length - 1)
            return itemId;

        var kind = itemId[..sep];
        var rawId = itemId[(sep + 1)..];

        return kind.ToLowerInvariant() switch
        {
            "asset" when Guid.TryParse(rawId, out var assetId) =>
                await db.Assets.AsNoTracking().Where(a => a.Id == assetId).Select(a => a.Name).FirstOrDefaultAsync(ct),
            "consumable" when int.TryParse(rawId, out var cId) =>
                await db.Consumables.AsNoTracking().Where(c => c.Id == cId).Select(c => c.Name).FirstOrDefaultAsync(ct),
            "component" when int.TryParse(rawId, out var pId) =>
                await db.Components.AsNoTracking().Where(c => c.Id == pId).Select(c => c.Name).FirstOrDefaultAsync(ct),
            "accessory" when int.TryParse(rawId, out var aId) =>
                await db.Accessories.AsNoTracking().Where(a => a.Id == aId).Select(a => a.Name).FirstOrDefaultAsync(ct),
            _ => itemId
        };
    }
}
