using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Services;

public sealed class SearchResultCaching(IProductSearchEngine inner, IMemoryCache cache) : IProductSearchEngine
{
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);
    private CancellationTokenSource _cts = new();

    private static string ByNameKey(string name) => $"search:name:{name}";
    private static string ByCategoryKey(int categoryId) => $"search:category:{categoryId}";
    private static string CompositeKey(string? name, int? categoryId) => $"search:name:{name}:cat:{categoryId}";

    public Task<IEnumerable<ProductDto>> SearchByNameAsync(string name) =>
        GetOrCreateAsync(ByNameKey(name), () => inner.SearchByNameAsync(name));

    public Task<IEnumerable<ProductDto>> SearchByCategoryAsync(int categoryId) =>
        GetOrCreateAsync(ByCategoryKey(categoryId), () => inner.SearchByCategoryAsync(categoryId));

    public Task<IEnumerable<ProductDto>> SearchAsync(string? name, int? categoryId) =>
        GetOrCreateAsync(CompositeKey(name, categoryId), () => inner.SearchAsync(name, categoryId));

    // Cancels all cached entries by expiring the shared token.
    public void Invalidate()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
    }

    private async Task<IEnumerable<ProductDto>> GetOrCreateAsync(
        string key, Func<Task<IEnumerable<ProductDto>>> factory)
    {
        if (cache.TryGetValue(key, out IEnumerable<ProductDto>? cached))
            return cached!;

        var result = await factory();

        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(DefaultTtl)
            .AddExpirationToken(new CancellationChangeToken(_cts.Token));

        cache.Set(key, result, options);
        return result;
    }
}
