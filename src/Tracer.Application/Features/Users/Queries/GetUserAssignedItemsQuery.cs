using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Users.DTOs;

namespace Tracer.Application.Features.Users.Queries;

public sealed record GetUserAssignedItemsQuery(Guid UserId) : IRequest<UserAssignedItemsDto?>;

public sealed class GetUserAssignedItemsQueryHandler
    : IRequestHandler<GetUserAssignedItemsQuery, UserAssignedItemsDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;

    public GetUserAssignedItemsQueryHandler(
        IApplicationDbContext context,
        IUserRepository users,
        ICurrentUserService currentUser)
    {
        _context = context;
        _users = users;
        _currentUser = currentUser;
    }

    public async Task<UserAssignedItemsDto?> Handle(
        GetUserAssignedItemsQuery request,
        CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context is available for the current user.");

        var user = await _users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || user.CompanyId != companyId)
            return null;

        var assets = await _context.Assets
            .AsNoTracking()
            .Where(a => a.AssignedUserId == request.UserId && a.CompanyId == companyId)
            .OrderBy(a => a.Name)
            .Select(a => new AssignedItemDto
            {
                ItemType = "Asset",
                Id = a.Id.ToString(),
                Name = a.Name,
                Identifier = a.AssetTag,
                AssignedAtUtc = a.CheckedOutAtUtc,
                Status = a.Status.ToString(),
                DetailPath = "/assets/" + a.Id
            })
            .ToListAsync(cancellationToken);

        var consumables = await _context.Consumables
            .AsNoTracking()
            .Where(c => c.AssignedUserId == request.UserId && c.CompanyId == companyId)
            .OrderBy(c => c.Name)
            .Select(c => new AssignedItemDto
            {
                ItemType = "Consumable",
                Id = c.Id.ToString(),
                Name = c.Name,
                Identifier = null,
                AssignedAtUtc = c.AssignedAtUtc,
                Status = c.TotalQuantity <= c.ReorderThreshold ? "Reorder" : "InStock",
                DetailPath = null
            })
            .ToListAsync(cancellationToken);

        var components = await _context.Components
            .AsNoTracking()
            .Where(c => c.AssignedUserId == request.UserId && c.CompanyId == companyId)
            .OrderBy(c => c.Name)
            .Select(c => new AssignedItemDto
            {
                ItemType = "Component",
                Id = c.Id.ToString(),
                Name = c.Name,
                Identifier = null,
                AssignedAtUtc = c.AssignedAtUtc,
                Status = null,
                DetailPath = null
            })
            .ToListAsync(cancellationToken);

        var accessories = await _context.Accessories
            .AsNoTracking()
            .Where(a => a.AssignedUserId == request.UserId && a.CompanyId == companyId)
            .OrderBy(a => a.Name)
            .Select(a => new AssignedItemDto
            {
                ItemType = "Accessory",
                Id = a.Id.ToString(),
                Name = a.Name,
                Identifier = null,
                AssignedAtUtc = a.AssignedAtUtc,
                Status = null,
                DetailPath = null
            })
            .ToListAsync(cancellationToken);

        var licenses = await (
            from seat in _context.LicenseSeats.AsNoTracking()
            join license in _context.SoftwareLicenses.AsNoTracking()
                on seat.SoftwareLicenseId equals license.Id
            where seat.AssignedUserId == request.UserId && license.CompanyId == companyId
            orderby license.Name
            select new AssignedItemDto
            {
                ItemType = "License",
                Id = license.Id.ToString(),
                Name = license.Name,
                Identifier = seat.Id.ToString(),
                AssignedAtUtc = seat.CreatedAtUtc,
                Status = license.ExpirationDate.HasValue && license.ExpirationDate < DateTime.UtcNow
                    ? "Expired"
                    : "Active",
                DetailPath = null
            })
            .ToListAsync(cancellationToken);

        return new UserAssignedItemsDto
        {
            Assets = assets,
            Consumables = consumables,
            Components = components,
            Accessories = accessories,
            Licenses = licenses
        };
    }
}
