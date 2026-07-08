using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.DepreciationAggregate;

/// <summary>
/// Depreciation aggregate root.
/// Represents a schedule used to calculate the current value of an asset over time.
/// </summary>
public sealed class Depreciation : AuditableEntity<Guid>
{
    private Depreciation() { }

    private Depreciation(Guid id, string name, int months, Guid companyId, decimal minimumValue)
        : base(id)
    {
        Name = name;
        Months = months;
        CompanyId = companyId;
        MinimumValue = minimumValue;
    }

    public string Name { get; private set; } = string.Empty;
    public int Months { get; private set; }
    public Guid CompanyId { get; private set; }
    public decimal MinimumValue { get; private set; }

    public static Depreciation Create(string name, int months, Guid companyId, decimal minimumValue)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (months <= 0)
            throw new ArgumentException("Months must be greater than zero.", nameof(months));
        if (minimumValue < 0)
            throw new ArgumentException("Minimum value cannot be negative.", nameof(minimumValue));

        return new Depreciation(Guid.NewGuid(), name.Trim(), months, companyId, minimumValue);
    }
}
