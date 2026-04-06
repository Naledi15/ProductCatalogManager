using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ProductCatalogManager.Domain.Helpers;

/// <summary>
/// A reusable in-memory cache group with shared token-based invalidation.
/// Register one instance per logical cache group (e.g. products, categories).
/// All entries created by this layer are evicted together when Invalidate() is called.
/// </summary>
public sealed class CacheLayer(IMemoryCache cache)
{
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);
    private CancellationTokenSource _cts = new();

    /// <summary>Returns a cached value, or invokes <paramref name="factory"/> to compute and store it.</summary>
    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory) where T : class
    {
        if (cache.TryGetValue(key, out T? hit))
            return hit!;

        var value = await factory();

        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(DefaultTtl)
            .AddExpirationToken(new CancellationChangeToken(_cts.Token));

        cache.Set(key, value, options);
        return value;
    }

    /// <summary>Evicts all entries belonging to this cache group.</summary>
    public void Invalidate()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
    }
}
