using Tracer.Application.Features.Assets.DTOs;
using Tracer.Domain.Aggregates.AssetAggregate;

namespace Tracer.Application.Features.Assets.Mapping;

/// <summary>
/// Manual entity → DTO projections. Explicit mapping keeps the read contract stable and avoids
/// reflection-based mapping overhead on hot query paths (Doc 10 DTO layer).
/// </summary>
public static class AssetMappingExtensions
{
    public static AssetDto ToDto(this Asset asset) => new()
    {
        Id = asset.Id,
        AssetTag = asset.AssetTag,
        Name = asset.Name,
        SerialNumber = asset.SerialNumber,
        Status = asset.Status.ToString(),
        AssetModelId = asset.AssetModelId,
        StatusLabelId = asset.StatusLabelId,
        LocationId = asset.LocationId,
        AssignedUserId = asset.AssignedUserId,
        PurchaseCost = asset.PurchaseCost,
        CheckedOutAtUtc = asset.CheckedOutAtUtc
    };

    public static AssetDetailDto ToDetailDto(this Asset asset) => new()
    {
        Id = asset.Id,
        AssetTag = asset.AssetTag,
        Name = asset.Name,
        SerialNumber = asset.SerialNumber,
        Status = asset.Status.ToString(),
        Notes = asset.Notes,
        CompanyId = asset.CompanyId,
        AssetModelId = asset.AssetModelId,
        StatusLabelId = asset.StatusLabelId,
        LocationId = asset.LocationId,
        AssignedUserId = asset.AssignedUserId,
        PurchaseCost = asset.PurchaseCost,
        PurchaseDate = asset.PurchaseDate,
        CheckedOutAtUtc = asset.CheckedOutAtUtc,
        LastCheckinAtUtc = asset.LastCheckinAtUtc,
        CreatedAtUtc = asset.CreatedAtUtc,
        CreatedBy = asset.CreatedBy,
        UpdatedAtUtc = asset.UpdatedAtUtc,
        UpdatedBy = asset.UpdatedBy
    };
}
