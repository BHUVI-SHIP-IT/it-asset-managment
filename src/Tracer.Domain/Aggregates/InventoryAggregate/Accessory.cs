using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.InventoryAggregate;

/// <summary>
/// Accessory aggregate root (Doc 4 §3.15).
/// </summary>
public sealed class Accessory : AuditableEntity<int>
{
    private Accessory() { }

    private Accessory(
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

    public static Accessory Create(string name, Guid companyId, int totalQuantity, decimal purchaseCost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (totalQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(totalQuantity));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        return new Accessory(name.Trim(), companyId, totalQuantity, purchaseCost);
    }

    public void Update(string name, int totalQuantity, decimal purchaseCost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (totalQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(totalQuantity));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        Name = name.Trim();
        TotalQuantity = totalQuantity;
        PurchaseCost = purchaseCost;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }
}
