namespace Tracer.Application.Features.Assets.DTOs;

/// <summary>Full read model for the asset detail page (Doc 9 §4.1 AssetDetailDto).</summary>
public sealed record AssetDetailDto
{
    public Guid Id { get; init; }
    public string AssetTag { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? SerialNumber { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }

    public Guid CompanyId { get; init; }
    public Guid AssetModelId { get; init; }
    public int StatusLabelId { get; init; }
    public Guid? LocationId { get; init; }
    public Guid? AssignedUserId { get; init; }

    public decimal PurchaseCost { get; init; }
    public DateTime? PurchaseDate { get; init; }
    public DateTime? CheckedOutAtUtc { get; init; }
    public DateTime? LastCheckinAtUtc { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public Guid? UpdatedBy { get; init; }
}
