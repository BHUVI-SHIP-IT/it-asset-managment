using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.LicenseAggregate;

/// <summary>
/// SoftwareLicense aggregate root (Doc 4 §3.13).
/// Manages enterprise software licenses and available seats.
/// </summary>
public sealed class SoftwareLicense : AuditableEntity<Guid>
{
    private SoftwareLicense() { }

    private SoftwareLicense(
        Guid id,
        string name,
        Guid companyId,
        Guid? manufacturerId,
        int totalSeats,
        decimal purchaseCost,
        DateTime? expirationDate) : base(id)
    {
        Name = name;
        CompanyId = companyId;
        ManufacturerId = manufacturerId;
        TotalSeats = totalSeats;
        PurchaseCost = purchaseCost;
        ExpirationDate = expirationDate;
    }

    public string Name { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid? ManufacturerId { get; private set; }
    public int TotalSeats { get; private set; }
    public decimal PurchaseCost { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public string? Notes { get; private set; }

        public static SoftwareLicense Create(
        string name,
        Guid companyId,
        Guid? manufacturerId,
        int totalSeats,
        decimal purchaseCost,
        DateTime? expirationDate = null)
        => Create(Guid.NewGuid(), name, companyId, manufacturerId, totalSeats, purchaseCost, expirationDate);

    public static SoftwareLicense Create(
        Guid id,
        string name,
        Guid companyId,
        Guid? manufacturerId,
        int totalSeats,
        decimal purchaseCost,
        DateTime? expirationDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("License name is required.", nameof(name));
        if (totalSeats < 1)
            throw new ArgumentException("Total seats must be at least 1.", nameof(totalSeats));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        return new SoftwareLicense(
            id, name.Trim(), companyId, manufacturerId, totalSeats, purchaseCost, expirationDate);
    }

    public void Update(
        string name,
        Guid? manufacturerId,
        int totalSeats,
        decimal purchaseCost,
        DateTime? expirationDate,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("License name is required.", nameof(name));
        if (totalSeats < 1)
            throw new ArgumentException("Total seats must be at least 1.", nameof(totalSeats));
        if (purchaseCost < 0)
            throw new ArgumentException("Purchase cost cannot be negative.", nameof(purchaseCost));

        Name = name.Trim();
        ManufacturerId = manufacturerId;
        TotalSeats = totalSeats;
        PurchaseCost = purchaseCost;
        ExpirationDate = expirationDate;
        Notes = notes?.Trim();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Extends or sets the expiration date (used by license renewal approvals).</summary>
    public void ExtendExpiration(DateTime newExpirationUtc)
    {
        if (newExpirationUtc.Kind == DateTimeKind.Unspecified)
            newExpirationUtc = DateTime.SpecifyKind(newExpirationUtc, DateTimeKind.Utc);

        if (ExpirationDate is not null && newExpirationUtc <= ExpirationDate)
            throw new ArgumentException("New expiration must be after the current expiration.", nameof(newExpirationUtc));

        ExpirationDate = newExpirationUtc;
    }
}
