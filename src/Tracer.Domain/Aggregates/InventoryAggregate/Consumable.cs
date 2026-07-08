using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.InventoryAggregate;

/// <summary>
/// Consumable aggregate root (Doc 4 §3.17).
/// Manages consumable items and handles FIFO checkout logic.
/// </summary>
public sealed class Consumable : AuditableEntity<int>
{
    private Consumable() { }

    private Consumable(
        string name,
        Guid companyId,
        int totalQuantity,
        decimal purchaseCost)
    {
        Name = name;
        CompanyId = companyId;
        TotalQuantity = totalQuantity;
        PurchaseCost = purchaseCost;
    }

    public string Name { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public int TotalQuantity { get; private set; }
    public decimal PurchaseCost { get; private set; }

    public static Consumable Create(string name, Guid companyId, int totalQuantity, decimal purchaseCost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (totalQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(totalQuantity));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        return new Consumable(name.Trim(), companyId, totalQuantity, purchaseCost);
    }

    /// <summary>
    /// Checks out a quantity of this consumable, ensuring stock doesn't fall below zero.
    /// </summary>
    public void Checkout(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Checkout quantity must be greater than zero.", nameof(quantity));

        if (TotalQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock. Requested: {quantity}, Available: {TotalQuantity}");

        TotalQuantity -= quantity;
        
        // In a real scenario we'd raise a ConsumableCheckedOutEvent here
    }
}
