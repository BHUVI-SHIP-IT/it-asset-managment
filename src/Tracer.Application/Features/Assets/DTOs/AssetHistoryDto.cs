namespace Tracer.Application.Features.Assets.DTOs;

/// <summary>
/// One version of an asset from SQL Server temporal history (Assets + AssetsHistory).
/// <see cref="ValidFrom"/> / <see cref="ValidTo"/> map to PeriodStart / PeriodEnd.
/// </summary>
public sealed record AssetHistoryDto
{
    public Guid Id { get; init; }
    public string AssetTag { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int StatusLabelId { get; init; }
    public Guid? AssignedUserId { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime ValidTo { get; init; }
}
