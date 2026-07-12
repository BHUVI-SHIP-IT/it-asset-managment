using Tracer.Domain.Common;

namespace Tracer.Domain.Aggregates.DepreciationAggregate;

public enum DepreciationMethod
{
    StraightLine = 0
}

/// <summary>
/// Depreciation schedule used to calculate current asset value over time.
/// </summary>
public sealed class Depreciation : AuditableEntity<Guid>
{
    private Depreciation() { }

    private Depreciation(
        Guid id,
        string name,
        int months,
        Guid companyId,
        decimal minimumValue,
        DepreciationMethod method)
        : base(id)
    {
        Name = name;
        Months = months;
        CompanyId = companyId;
        MinimumValue = minimumValue;
        Method = method;
    }

    public string Name { get; private set; } = string.Empty;
    /// <summary>Useful life in months (years = Months / 12).</summary>
    public int Months { get; private set; }
    public Guid CompanyId { get; private set; }
    /// <summary>Salvage / residual value floor.</summary>
    public decimal MinimumValue { get; private set; }
    public DepreciationMethod Method { get; private set; }

    public int UsefulLifeYears => Math.Max(1, (int)Math.Round(Months / 12.0));

    public static Depreciation Create(
        string name,
        int months,
        Guid companyId,
        decimal minimumValue,
        DepreciationMethod method = DepreciationMethod.StraightLine)
        => Create(Guid.NewGuid(), name, months, companyId, minimumValue, method);

    public static Depreciation Create(
        Guid id,
        string name,
        int months,
        Guid companyId,
        decimal minimumValue,
        DepreciationMethod method = DepreciationMethod.StraightLine)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (months <= 0)
            throw new ArgumentException("Months must be greater than zero.", nameof(months));
        if (minimumValue < 0)
            throw new ArgumentException("Minimum value cannot be negative.", nameof(minimumValue));

        return new Depreciation(id, name.Trim(), months, companyId, minimumValue, method);
    }

    /// <summary>Straight-line book value as of <paramref name="asOfUtc"/>.</summary>
    public decimal ComputeCurrentValue(decimal purchaseCost, DateTime? purchaseDate, DateTime asOfUtc)
    {
        if (purchaseCost < 0)
            return MinimumValue;
        if (purchaseDate is null)
            return purchaseCost;

        var elapsedMonths = ((asOfUtc.Year - purchaseDate.Value.Year) * 12)
            + asOfUtc.Month - purchaseDate.Value.Month;
        if (elapsedMonths <= 0)
            return purchaseCost;

        var depreciable = Math.Max(0, purchaseCost - MinimumValue);
        var perMonth = depreciable / Months;
        var book = purchaseCost - (perMonth * Math.Min(elapsedMonths, Months));
        return Math.Max(MinimumValue, Math.Round(book, 2));
    }
}
