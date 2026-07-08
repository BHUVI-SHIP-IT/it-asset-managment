using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.InventoryAggregate;

/// <summary>
/// Component aggregate root (Doc 4 §3.16).
/// </summary>
public sealed class Component : AuditableEntity<int>
{
    private Component() { }

    private Component(
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

    public static Component Create(string name, Guid companyId, int totalQuantity, decimal purchaseCost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (totalQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.", nameof(totalQuantity));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        return new Component(name.Trim(), companyId, totalQuantity, purchaseCost);
    }
}
