using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Features.Requests.DTOs;
using Tracer.Domain.Aggregates.RequestAggregate;
using Tracer.Shared.Results;

namespace Tracer.Application.Features.Requests.Commands;

public sealed record CreateRequestCommand(
    string Type,
    string ItemId,
    int? Quantity,
    string? Notes) : IRequest<Result<Guid>>;

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
            await ValidateItemExistsAsync(type, request.ItemId, companyId, cancellationToken);

            var entity = InventoryRequest.Create(
                companyId,
                type,
                userId,
                request.ItemId,
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
        }
    }
}
