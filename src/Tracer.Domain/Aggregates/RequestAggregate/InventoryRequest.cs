using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.RequestAggregate;

public enum RequestType
{
    Asset = 0,
    Consumable = 1,
    Component = 2,
    Accessory = 3,
    LicenseRenewal = 4,
    Return = 5
}

public enum RequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

/// <summary>
/// Inventory / license request awaiting approval.
/// </summary>
public sealed class InventoryRequest : AuditableEntity<Guid>
{
    private InventoryRequest() { }

    private InventoryRequest(
        Guid id,
        Guid companyId,
        RequestType type,
        Guid requestedByUserId,
        string? itemId,
        int? quantity,
        string? notes) : base(id)
    {
        CompanyId = companyId;
        Type = type;
        RequestedByUserId = requestedByUserId;
        ItemId = itemId;
        Quantity = quantity;
        Notes = notes;
        Status = RequestStatus.Pending;
        RequestedAtUtc = DateTime.UtcNow;
    }

    public Guid CompanyId { get; private set; }
    public RequestType Type { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    /// <summary>Asset/license Guid or consumable/component/accessory int id, as string.</summary>
    public string? ItemId { get; private set; }
    public int? Quantity { get; private set; }
    public RequestStatus Status { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public Guid? ResolvedByUserId { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }
    public string? Notes { get; private set; }
    public string? ResolutionNotes { get; private set; }

    public static InventoryRequest Create(
        Guid companyId,
        RequestType type,
        Guid requestedByUserId,
        string? itemId,
        int? quantity,
        string? notes)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company is required.", nameof(companyId));
        if (requestedByUserId == Guid.Empty)
            throw new ArgumentException("Requester is required.", nameof(requestedByUserId));

        if (type == RequestType.Consumable)
        {
            if (quantity is null or <= 0)
                throw new ArgumentException("Quantity is required for consumable requests.", nameof(quantity));
        }

        if (string.IsNullOrWhiteSpace(itemId))
            throw new ArgumentException("Item is required.", nameof(itemId));

        return new InventoryRequest(
            Guid.NewGuid(),
            companyId,
            type,
            requestedByUserId,
            itemId.Trim(),
            quantity,
            notes?.Trim());
    }

    public void Approve(Guid resolverUserId, string? resolutionNotes = null)
    {
        EnsurePending();
        Status = RequestStatus.Approved;
        ResolvedByUserId = resolverUserId;
        ResolvedAtUtc = DateTime.UtcNow;
        ResolutionNotes = resolutionNotes?.Trim();
    }

    public void Reject(Guid resolverUserId, string? resolutionNotes = null)
    {
        EnsurePending();
        Status = RequestStatus.Rejected;
        ResolvedByUserId = resolverUserId;
        ResolvedAtUtc = DateTime.UtcNow;
        ResolutionNotes = resolutionNotes?.Trim();
    }

    private void EnsurePending()
    {
        if (Status != RequestStatus.Pending)
            throw new InvalidOperationException($"Request is already {Status}.");
    }
}
