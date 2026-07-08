using Tracer.Domain.Common;
using Tracer.Domain.Events;
using Tracer.Domain.Exceptions;

namespace Tracer.Domain.Aggregates.AssetAggregate;

/// <summary>
/// Asset aggregate root (Doc 4 Assets table, Doc 10 §7.4). Encapsulates all lifecycle
/// invariants; state may only change through these methods, never via public setters,
/// so the checkout/checkin state machine (Doc 3 §4.2) cannot be violated.
/// </summary>
public sealed class Asset : AuditableEntity<Guid>
{
    private Asset() { } // EF Core

    private Asset(
        Guid id,
        string assetTag,
        string name,
        Guid companyId,
        Guid assetModelId,
        int statusLabelId,
        Guid? locationId,
        decimal purchaseCost,
        string? serialNumber,
        DateTime? purchaseDate,
        Guid? depreciationId = null) : base(id)
    {
        AssetTag = assetTag;
        Name = name;
        CompanyId = companyId;
        AssetModelId = assetModelId;
        StatusLabelId = statusLabelId;
        LocationId = locationId;
        PurchaseCost = purchaseCost;
        SerialNumber = serialNumber;
        PurchaseDate = purchaseDate;
        DepreciationId = depreciationId;
        CurrentValue = purchaseCost;
        Status = AssetStatus.Deployable;
    }

    public string AssetTag { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? SerialNumber { get; private set; }
    public AssetStatus Status { get; private set; }

    public Guid CompanyId { get; private set; }
    public Guid AssetModelId { get; private set; }
    public int StatusLabelId { get; private set; }
    public Guid? LocationId { get; private set; }

    public Guid? AssignedUserId { get; private set; }
    public DateTime? CheckedOutAtUtc { get; private set; }
    public DateTime? LastCheckinAtUtc { get; private set; }

    public decimal PurchaseCost { get; private set; }
    public DateTime? PurchaseDate { get; private set; }
    
    public Guid? DepreciationId { get; private set; }
    public decimal CurrentValue { get; private set; }
    public Tracer.Domain.Aggregates.DepreciationAggregate.Depreciation? Depreciation { get; private set; }

    public string? Notes { get; private set; }

    /// <summary>Factory for registering a new asset (Doc 8 §2.1). Starts as Deployable.</summary>
    public static Asset Create(
        string assetTag,
        string name,
        Guid companyId,
        Guid assetModelId,
        int statusLabelId,
        decimal purchaseCost,
        Guid? locationId = null,
        string? serialNumber = null,
        DateTime? purchaseDate = null,
        Guid? depreciationId = null)
    {
        if (string.IsNullOrWhiteSpace(assetTag))
            throw new ArgumentException("Asset tag is required.", nameof(assetTag));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Asset name is required.", nameof(name));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        var asset = new Asset(
            Guid.NewGuid(), assetTag.Trim(), name.Trim(), companyId, assetModelId,
            statusLabelId, locationId, purchaseCost, serialNumber?.Trim(), purchaseDate, depreciationId);

        asset.RaiseDomainEvent(new AssetCreatedDomainEvent(asset.Id, asset.AssetTag));
        return asset;
    }

    /// <summary>Updates mutable descriptive fields. Does not alter lifecycle state.</summary>
    public void UpdateDetails(
        string name,
        Guid assetModelId,
        int statusLabelId,
        decimal purchaseCost,
        Guid? locationId,
        string? serialNumber,
        DateTime? purchaseDate,
        Guid? depreciationId,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Asset name is required.", nameof(name));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        Name = name.Trim();
        AssetModelId = assetModelId;
        StatusLabelId = statusLabelId;
        PurchaseCost = purchaseCost;
        LocationId = locationId;
        SerialNumber = serialNumber?.Trim();
        PurchaseDate = purchaseDate;
        DepreciationId = depreciationId;
        Notes = notes?.Trim();
    }
    
    public void UpdateCurrentValue(decimal newValue)
    {
        if (newValue < 0)
            throw new ArgumentException("Value cannot be negative.", nameof(newValue));
        CurrentValue = newValue;
    }

    /// <summary>
    /// Checks the asset out to a user (Doc 3 §4.2). Invariant: status must be Deployable.
    /// </summary>
    public void Checkout(Guid userId)
    {
        if (Status != AssetStatus.Deployable)
            throw new AssetNotDeployableException(Id, Status.ToString());

        AssignedUserId = userId;
        Status = AssetStatus.Deployed;
        CheckedOutAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new AssetCheckedOutEvent(Id, userId, CheckedOutAtUtc.Value));
    }

    /// <summary>
    /// Returns the asset to inventory (Doc 3 §4.2). Invariant: must currently be assigned.
    /// </summary>
    public void Checkin()
    {
        if (Status != AssetStatus.Deployed || AssignedUserId is null)
            throw new AssetAlreadyCheckedInException(Id);

        var previousUserId = AssignedUserId.Value;
        AssignedUserId = null;
        Status = AssetStatus.Deployable;
        LastCheckinAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new AssetCheckedInEvent(Id, previousUserId, LastCheckinAtUtc.Value));
    }

    /// <summary>Transfers a deployed asset directly from one user to another (Doc 5 transfer endpoint).</summary>
    public void Transfer(Guid newUserId)
    {
        if (Status != AssetStatus.Deployed || AssignedUserId is null)
            throw new AssetAlreadyCheckedInException(Id);

        var previousUserId = AssignedUserId.Value;
        AssignedUserId = newUserId;
        CheckedOutAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new AssetCheckedInEvent(Id, previousUserId, DateTime.UtcNow));
        RaiseDomainEvent(new AssetCheckedOutEvent(Id, newUserId, CheckedOutAtUtc.Value));
    }

    /// <summary>Retires the asset (Doc 7 Assets.Archive). Terminal; a deployed asset must be checked in first.</summary>
    public void Retire()
    {
        if (Status == AssetStatus.Deployed)
            throw new AssetAlreadyCheckedInException(Id);

        Status = AssetStatus.Archived;
        AssignedUserId = null;
    }
}
