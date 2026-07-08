using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Tracer.Application.Common.Interfaces;

namespace Tracer.Infrastructure.Cache;

/// <summary>
/// Redis-backed implementation of <see cref="ICacheService"/> using
/// <see cref="IDistributedCache"/> with JSON serialisation.
/// Falls back gracefully when Redis is unavailable — the factory is always invoked
/// and the result is returned without caching rather than throwing.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan absoluteExpiration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            if (cached is not null)
                return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
        }
        catch
        {
            // Redis unavailable — fall through to factory call.
        }

        var value = await factory(cancellationToken);

        if (value is not null)
        {
            try
            {
                var serialised = JsonSerializer.Serialize(value, _jsonOptions);
                await _cache.SetStringAsync(key, serialised, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration
                }, cancellationToken);
            }
            catch
            {
                // Redis unavailable — return value without caching.
            }
        }

        return value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch
        {
            // Best-effort removal.
        }
    }
}
