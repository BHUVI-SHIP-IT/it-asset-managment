using Tracer.Application.Common.Specifications;
using Tracer.Domain.Aggregates.AssetAggregate;

namespace Tracer.Application.Common.Interfaces;

/// <summary>
/// Aggregate-root repository for Asset (Doc 10 §4.3 "Only Aggregate Roots have repositories").
/// Query shaping is delegated to specifications.
/// </summary>
public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Asset>> ListAsync(ISpecification<Asset> specification, CancellationToken cancellationToken = default);

    Task<int> CountAsync(ISpecification<Asset> specification, CancellationToken cancellationToken = default);

    Task<bool> AssetTagExistsAsync(string assetTag, Guid companyId, Guid? excludeAssetId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);

    void Update(Asset asset);

    void Remove(Asset asset);
}
