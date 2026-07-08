namespace Tracer.Application.Common.Interfaces;

/// <summary>
/// Distributed cache abstraction — hides <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>
/// serialisation details and provides a convenient cache-aside helper.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Returns the cached value for <paramref name="key"/> if present; otherwise invokes
    /// <paramref name="factory"/>, caches the result for <paramref name="absoluteExpiration"/>,
    /// and returns it.
    /// </summary>
    Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan absoluteExpiration,
        CancellationToken cancellationToken = default);

    /// <summary>Removes a cache entry by key.</summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
