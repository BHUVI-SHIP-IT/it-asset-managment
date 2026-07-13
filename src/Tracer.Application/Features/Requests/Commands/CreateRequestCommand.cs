using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.RequestAggregate;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Requests.Commands;

public sealed record CreateRequestCommand(
    string Type,
    string ItemId,
    int? Quantity,
    string? Notes,
    string? ItemKind = null) : IRequest<Result<Guid>>;

public sealed class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

public sealed class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, Result<Guid>>
{
    private static readonly HashSet<string> ReturnKinds = new(StringComparer.OrdinalIgnoreCase)
    {
        "Asset", "Consumable", "Component", "Accessory"
    };

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateRequestCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context.");
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("No user context.");

        if (!Enum.TryParse<RequestType>(request.Type, ignoreCase: true, out var type))
            return Result.Failure<Guid>(Error.Validation("Request.InvalidType", "Invalid request type."));

        try
        {
            var storedItemId = request.ItemId.Trim();

            if (type == RequestType.Return)
            {
                if (string.IsNullOrWhiteSpace(request.ItemKind) || !ReturnKinds.Contains(request.ItemKind))
                    return Result.Failure<Guid>(Error.Validation(
                        "Request.InvalidItemKind",
                        "Return requests require itemKind of Asset, Consumable, Component, or Accessory."));

                storedItemId = $"{request.ItemKind.Trim()}:{request.ItemId.Trim()}";
                await ValidateReturnItemAsync(request.ItemKind.Trim(), request.ItemId.Trim(), companyId, userId, cancellationToken);

                var duplicate = await _db.InventoryRequests.AnyAsync(
                    r => r.CompanyId == companyId
                         && r.RequestedByUserId == userId
                         && r.Type == RequestType.Return
                         && r.Status == RequestStatus.Pending
                         && r.ItemId == storedItemId,
                    cancellationToken);
                if (duplicate)
                    return Result.Failure<Guid>(Error.Validation(
                        "Request.DuplicateReturn",
                        "A pending return request already exists for this item."));
            }
            else
            {
                await ValidateItemExistsAsync(type, request.ItemId, companyId, cancellationToken);
            }

            var entity = InventoryRequest.Create(
                companyId,
                type,
                userId,
                storedItemId,
                request.Quantity,
                request.Notes);

            _db.InventoryRequests.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success(entity.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(Error.Validation("Request.Invalid", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<Guid>(Error.Validation("Request.Invalid", ex.Message));
        }
    }

    private async Task ValidateReturnItemAsync(
        string kind, string itemId, Guid companyId, Guid userId, CancellationToken ct)
    {
        switch (kind.ToLowerInvariant())
        {
            case "asset":
                if (!Guid.TryParse(itemId, out var assetId) ||
                    !await _db.Assets.AnyAsync(
                        a => a.Id == assetId && a.CompanyId == companyId && a.AssignedUserId == userId, ct))
                    throw new InvalidOperationException("Assigned asset not found.");
                break;
            case "consumable":
                if (!int.TryParse(itemId, out var consumableId) ||
                    !await _db.Consumables.AnyAsync(
                        c => c.Id == consumableId && c.CompanyId == companyId && c.AssignedUserId == userId, ct))
                    throw new InvalidOperationException("Assigned consumable not found.");
                break;
            case "component":
                if (!int.TryParse(itemId, out var componentId) ||
                    !await _db.Components.AnyAsync(
                        c => c.Id == componentId && c.CompanyId == companyId && c.AssignedUserId == userId, ct))
                    throw new InvalidOperationException("Assigned component not found.");
                break;
            case "accessory":
                if (!int.TryParse(itemId, out var accessoryId) ||
                    !await _db.Accessories.AnyAsync(
                        a => a.Id == accessoryId && a.CompanyId == companyId && a.AssignedUserId == userId, ct))
                    throw new InvalidOperationException("Assigned accessory not found.");
                break;
            default:
                throw new InvalidOperationException("Invalid return item kind.");
        }
    }

    private async Task ValidateItemExistsAsync(
        RequestType type, string itemId, Guid companyId, CancellationToken ct)
    {
        switch (type)
        {
            case RequestType.Asset:
                if (!Guid.TryParse(itemId, out var assetId) ||
                    !await _db.Assets.AnyAsync(a => a.Id == assetId && a.CompanyId == companyId, ct))
                    throw new InvalidOperationException("Asset not found.");
                break;
            case RequestType.Consumable:
                if (!int.TryParse(itemId, out var consumableId) ||
                    !await _db.Consumables.AnyAsync(c => c.Id == consumableId && c.CompanyId == companyId, ct))
                    throw new InvalidOperationException("Consumable not found.");
                break;
            case RequestType.Component:
                if (!int.TryParse(itemId, out var componentId) ||
                    !await _db.Components.AnyAsync(c => c.Id == componentId && c.CompanyId == companyId, ct))
                    throw new InvalidOperationException("Component not found.");
                break;
            case RequestType.Accessory:
                if (!int.TryParse(itemId, out var accessoryId) ||
                    !await _db.Accessories.AnyAsync(a => a.Id == accessoryId && a.CompanyId == companyId, ct))
                    throw new InvalidOperationException("Accessory not found.");
                break;
            case RequestType.LicenseRenewal:
                if (!Guid.TryParse(itemId, out var licenseId) ||
                    !await _db.SoftwareLicenses.AnyAsync(l => l.Id == licenseId && l.CompanyId == companyId, ct))
                    throw new InvalidOperationException("License not found.");
                break;
            case RequestType.Return:
                throw new InvalidOperationException("Return requests must use the return validation path.");
        }
    }
}
