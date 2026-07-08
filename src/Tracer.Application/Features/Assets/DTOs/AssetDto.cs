namespace Tracer.Application.Features.Assets.DTOs;

/// <summary>Lightweight read model for asset list grids (Doc 9 §4.1 AssetDto, Doc 6 MatTable).</summary>
public sealed record AssetDto
{
    public Guid Id { get; init; }
    public string AssetTag { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? SerialNumber { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid AssetModelId { get; init; }
    public int StatusLabelId { get; init; }
    public Guid? LocationId { get; init; }
    public Guid? AssignedUserId { get; init; }
    public decimal PurchaseCost { get; init; }
    public DateTime? CheckedOutAtUtc { get; init; }
}
