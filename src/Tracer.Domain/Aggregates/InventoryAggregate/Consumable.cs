using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.InventoryAggregate;

/// <summary>
/// Consumable aggregate root (Doc 4 §3.17).
/// Manages consumable stock with reorder threshold and optional user assignment.
/// </summary>
public sealed class Consumable : AuditableEntity<int>
{
    private Consumable() { }

    private Consumable(
        string name,
        Guid companyId,
        int totalQuantity,
        decimal purchaseCost,
        int reorderThreshold)
    {
        Name = name;
        CompanyId = companyId;
        TotalQuantity = totalQuantity;
        PurchaseCost = purchaseCost;
        ReorderThreshold = reorderThreshold;
    }

    public string Name { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public int TotalQuantity { get; private set; }
    public decimal PurchaseCost { get; private set; }
    public int ReorderThreshold { get; private set; }
    public Guid? AssignedUserId { get; private set; }
    public DateTime? AssignedAtUtc { get; private set; }

    public static Consumable Create(
        string name,
        Guid companyId,
        int totalQuantity,
        decimal purchaseCost,
        int reorderThreshold = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (totalQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(totalQuantity));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));
        if (reorderThreshold < 0)
            throw new ArgumentException("Reorder threshold cannot be negative.", nameof(reorderThreshold));

        return new Consumable(name.Trim(), companyId, totalQuantity, purchaseCost, reorderThreshold);
    }

    public void Checkout(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Checkout quantity must be greater than zero.", nameof(quantity));

        if (TotalQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock. Requested: {quantity}, Available: {TotalQuantity}");

        TotalQuantity -= quantity;
    }

    public void AssignTo(Guid userId, int quantity)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required.", nameof(userId));

        Checkout(quantity);
        AssignedUserId = userId;
        AssignedAtUtc = DateTime.UtcNow;
    }
}
