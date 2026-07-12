using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Domain.Aggregates.RequestAggregate;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Requests.Commands;

public sealed record ResolveRequestCommand(
    Guid RequestId,
    bool Approve,
    string? ResolutionNotes) : IRequest<Result>;

public sealed class ResolveRequestCommandValidator : AbstractValidator<ResolveRequestCommand>
{
    public ResolveRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.ResolutionNotes).MaximumLength(2000);
    }
}

public sealed class ResolveRequestCommandHandler : IRequestHandler<ResolveRequestCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ResolveRequestCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ResolveRequestCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId
            ?? throw new UnauthorizedAccessException("No tenant context.");
        var resolverId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException("No user context.");

        var entity = await _db.InventoryRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId && r.CompanyId == companyId, cancellationToken);

        if (entity is null)
            return Result.Failure(Error.NotFound("Request.NotFound", "Request not found."));

        try
        {
            if (!request.Approve)
            {
                entity.Reject(resolverId, request.ResolutionNotes);
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            await ApplyApprovalSideEffectsAsync(entity, cancellationToken);
            entity.Approve(resolverId, request.ResolutionNotes);
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Validation("Request.Invalid", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation("Request.Invalid", ex.Message));
        }
    }

    private async Task ApplyApprovalSideEffectsAsync(InventoryRequest entity, CancellationToken ct)
    {
        var itemId = entity.ItemId
            ?? throw new InvalidOperationException("Request has no item.");

        switch (entity.Type)
        {
            case RequestType.Asset:
            {
                if (!Guid.TryParse(itemId, out var assetId))
                    throw new InvalidOperationException("Invalid asset id.");
                var asset = await _db.Assets.FirstOrDefaultAsync(a => a.Id == assetId && a.CompanyId == entity.CompanyId, ct)
                    ?? throw new InvalidOperationException("Asset not found.");
                if (asset.Status == AssetStatus.Deployed && asset.AssignedUserId == entity.RequestedByUserId)
                    break;
                if (asset.Status == AssetStatus.Deployed)
                    asset.Checkin();
                asset.Checkout(entity.RequestedByUserId);
                asset.UpdateDetails(
                    asset.Name,
                    asset.AssetModelId,
                    2, // Deployed status label
                    asset.PurchaseCost,
                    asset.LocationId,
                    asset.SerialNumber,
                    asset.PurchaseDate,
                    asset.DepreciationId,
                    asset.Notes);
                asset.ClearDomainEvents();
                break;
            }
            case RequestType.Consumable:
            {
                if (!int.TryParse(itemId, out var id))
                    throw new InvalidOperationException("Invalid consumable id.");
                var consumable = await _db.Consumables.FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == entity.CompanyId, ct)
                    ?? throw new InvalidOperationException("Consumable not found.");
                var qty = entity.Quantity ?? 1;
                consumable.AssignTo(entity.RequestedByUserId, qty);
                break;
            }
            case RequestType.Component:
            {
                if (!int.TryParse(itemId, out var id))
                    throw new InvalidOperationException("Invalid component id.");
                var component = await _db.Components.FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == entity.CompanyId, ct)
                    ?? throw new InvalidOperationException("Component not found.");
                component.AssignTo(entity.RequestedByUserId, entity.Quantity ?? 1);
                break;
            }
            case RequestType.Accessory:
            {
                if (!int.TryParse(itemId, out var id))
                    throw new InvalidOperationException("Invalid accessory id.");
                var accessory = await _db.Accessories.FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == entity.CompanyId, ct)
                    ?? throw new InvalidOperationException("Accessory not found.");
                accessory.AssignTo(entity.RequestedByUserId, entity.Quantity ?? 1);
                break;
            }
            case RequestType.LicenseRenewal:
            {
                if (!Guid.TryParse(itemId, out var licenseId))
                    throw new InvalidOperationException("Invalid license id.");
                var license = await _db.SoftwareLicenses.FirstOrDefaultAsync(l => l.Id == licenseId && l.CompanyId == entity.CompanyId, ct)
                    ?? throw new InvalidOperationException("License not found.");
                var baseDate = license.ExpirationDate is null || license.ExpirationDate < DateTime.UtcNow
                    ? DateTime.UtcNow
                    : license.ExpirationDate.Value;
                license.ExtendExpiration(baseDate.AddYears(1));
                break;
            }
        }
    }
}
