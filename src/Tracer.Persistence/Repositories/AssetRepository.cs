using Microsoft.EntityFrameworkCore;
using Tracer.Application.Common.Interfaces;
using Tracer.Application.Common.Specifications;
using Tracer.Domain.Aggregates.AssetAggregate;
using Tracer.Persistence.Contexts;

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
}
