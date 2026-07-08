using Tracer.Application.Common.Specifications;
using Tracer.Domain.Aggregates.AssetAggregate;

namespace Tracer.Application.Features.Assets.Specifications;

/// <summary>
/// Filters/sorts/pages assets for the list grid (Doc 2 FR-1.2 server-side processing).
/// Tenant-scoped by CompanyId (Doc 7 tenant isolation). Set <paramref name="forCount"/> to skip paging.
/// </summary>
public sealed class AssetsByFilterSpec : BaseSpecification<Asset>
{
    public AssetsByFilterSpec(
        Guid companyId,
        string? searchTerm,
        AssetStatus? status,
        int? statusLabelId,
        Guid? locationId,
        int page,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        bool forCount = false)
    {
        var term = searchTerm?.Trim().ToLower();

        SetCriteria(a =>
            a.CompanyId == companyId
            && (status == null || a.Status == status)
            && (statusLabelId == null || a.StatusLabelId == statusLabelId)
            && (locationId == null || a.LocationId == locationId)
            && (string.IsNullOrEmpty(term)
                || a.AssetTag.ToLower().Contains(term)
                || a.Name.ToLower().Contains(term)
                || (a.SerialNumber != null && a.SerialNumber.ToLower().Contains(term))));

        ApplyNoTracking();

        if (forCount)
            return;

        ApplySorting(sortBy, sortDescending);
        ApplyPaging((page - 1) * pageSize, pageSize);
    }

    private void ApplySorting(string? sortBy, bool descending)
    {
        switch (sortBy?.Trim().ToLowerInvariant())
        {
            case "name":
                if (descending) ApplyOrderByDescending(a => a.Name); else ApplyOrderBy(a => a.Name);
                break;
            case "status":
                if (descending) ApplyOrderByDescending(a => a.Status); else ApplyOrderBy(a => a.Status);
                break;
            case "purchasecost":
                if (descending) ApplyOrderByDescending(a => a.PurchaseCost); else ApplyOrderBy(a => a.PurchaseCost);
                break;
            case "assettag":
            default:
                if (descending) ApplyOrderByDescending(a => a.AssetTag); else ApplyOrderBy(a => a.AssetTag);
                break;
        }
    }
}
