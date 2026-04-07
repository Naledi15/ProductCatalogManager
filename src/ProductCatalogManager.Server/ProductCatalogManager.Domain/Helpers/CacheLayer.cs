namespace ProductCatalogManager.Domain.Helpers;

/// <summary>
/// A simple in-memory cache group backed by a Dictionary.
/// </summary>
public sealed class CacheLayer
{
    private readonly object _lock = new();
    private Dictionary<string, object> _store = new();

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory) where T : class
    {
        lock (_lock)
        {
            if (_store.TryGetValue(key, out var hit))
                return (T)hit;
        }

        var value = await factory();

        lock (_lock)
        {
            _store[key] = value;
        }

        return value;
    }

    /// <summary>Evicts all entries belonging to this cache group.</summary>
    public void Invalidate()
    {
        lock (_lock)
        {
            _store = new Dictionary<string, object>();
        }
    }
}
