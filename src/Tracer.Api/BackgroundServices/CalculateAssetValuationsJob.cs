using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Api.BackgroundServices;

/// <summary>
/// Hangfire background job that periodically calculates the current financial value
/// of all tracked Assets based on their assigned depreciation schedule.
/// </summary>
public class CalculateAssetValuationsJob
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CalculateAssetValuationsJob> _logger;

    public CalculateAssetValuationsJob(IApplicationDbContext context, ILogger<CalculateAssetValuationsJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting CalculateAssetValuationsJob...");

        // Only process assets that are active (not retired) and have a depreciation schedule assigned.
        var assetsToValuate = await _context.Assets
            .Include(a => a.Depreciation) // This requires navigation property in Asset!
            .Where(a => a.Status != Tracer.Domain.Aggregates.AssetAggregate.AssetStatus.Archived && a.DepreciationId != null)
            .ToListAsync(cancellationToken);

        int updatedCount = 0;

        foreach (var asset in assetsToValuate)
        {
            if (asset.PurchaseDate == null) continue;

            var depreciation = asset.Depreciation!; // We need to add this navigation prop to Asset.
            
            // Calculate elapsed months since purchase
            var monthsElapsed = ((DateTime.UtcNow.Year - asset.PurchaseDate.Value.Year) * 12) + 
                                DateTime.UtcNow.Month - asset.PurchaseDate.Value.Month;

            if (monthsElapsed < 0) monthsElapsed = 0;

            decimal newValue;
            if (monthsElapsed >= depreciation.Months)
            {
                // Fully depreciated
                newValue = depreciation.MinimumValue;
            }
            else
            {
                // Straight-line depreciation calculation
                decimal depreciationAmountPerMonth = (asset.PurchaseCost - depreciation.MinimumValue) / depreciation.Months;
                newValue = asset.PurchaseCost - (depreciationAmountPerMonth * monthsElapsed);
                if (newValue < depreciation.MinimumValue) newValue = depreciation.MinimumValue;
            }

            if (asset.CurrentValue != newValue)
            {
                asset.UpdateCurrentValue(newValue);
                updatedCount++;
            }
        }

        if (updatedCount > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation($"CalculateAssetValuationsJob completed. Updated {updatedCount} assets.");
    }
}
