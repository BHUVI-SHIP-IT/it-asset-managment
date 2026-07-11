using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Common.Specifications;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Persistence.Contexts;
using Tracer.Application.Features.Assets.DTOs;

namespace Tracer.Persistence.Repositories;

/// <summary>
/// Concrete repository for the Asset aggregate root (Doc 10 §4.3).
/// Uses the Specification pattern to keep query construction declarative and testable.
/// </summary>
public sealed class AssetRepository : IAssetRepository
{
    private readonly TracerDbContext _context;

    public AssetRepository(TracerDbContext context) => _context = context;

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> ListAsync(
        ISpecification<Asset> specification,
        CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator
            .GetQuery(_context.Assets.AsQueryable(), specification)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        ISpecification<Asset> specification,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Assets.AsQueryable();

        if (specification.Criteria is not null)
            query = query.Where(specification.Criteria);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> AssetTagExistsAsync(
        string assetTag,
        Guid companyId,
        Guid? excludeAssetId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Assets
            .Where(a => a.AssetTag == assetTag && a.CompanyId == companyId);

        if (excludeAssetId.HasValue)
            query = query.Where(a => a.Id != excludeAssetId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(asset, cancellationToken);
    }

    public void Update(Asset asset)
    {
        _context.Assets.Update(asset);
    }

    public void Remove(Asset asset)
    {
        _context.Assets.Remove(asset);
    }

    public async Task<IReadOnlyList<AssetHistoryDto>> GetHistoryAsync(
        Guid assetId, CancellationToken cancellationToken = default)
    {
        // Any PeriodEnd in year 9999 is treated as the open (current) temporal row.
        const int OpenPeriodYear = 9999;

        // TemporalAll = current row + AssetsHistory. Exclude the open current period so a
        // brand-new asset (never updated) returns [] — prior versions only.
        // Project to anonymous type first: enum.ToString() is not always SQL-translatable.
        var rows = await _context.Assets
            .TemporalAll()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(a => a.Id == assetId)
            .Where(a => EF.Property<DateTime>(a, "PeriodEnd").Year < OpenPeriodYear)
            .OrderByDescending(a => EF.Property<DateTime>(a, "PeriodStart"))
            .Select(a => new
            {
                a.Id,
                a.AssetTag,
                a.Name,
                a.Status,
                a.StatusLabelId,
                a.AssignedUserId,
                ValidFrom = EF.Property<DateTime>(a, "PeriodStart"),
                ValidTo = EF.Property<DateTime>(a, "PeriodEnd")
            })
            .ToListAsync(cancellationToken);

        return rows
            .Select(a => new AssetHistoryDto
            {
                Id = a.Id,
                AssetTag = a.AssetTag,
                Name = a.Name,
                Status = a.Status.ToString(),
                StatusLabelId = a.StatusLabelId,
                AssignedUserId = a.AssignedUserId,
                ValidFrom = a.ValidFrom,
                ValidTo = a.ValidTo
            })
            .ToList();
    }
}
