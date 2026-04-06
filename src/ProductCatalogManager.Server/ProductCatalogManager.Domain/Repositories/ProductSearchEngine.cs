using Microsoft.Extensions.DependencyInjection;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Helpers;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public sealed class ProductSearchEngine(
    IProductRepository repository,
    [FromKeyedServices("products")] CacheLayer cache) : IProductSearchEngine
{
    private static string ByNameKey(string name) => $"search:name:{name}";
    private static string ByCategoryKey(int categoryId) => $"search:category:{categoryId}";
    private static string ByNameAndCategoryKey(string? name, int? categoryId) => $"search:name:{name}:cat:{categoryId}";

    public Task<IEnumerable<ProductDto>> SearchByNameAsync(string name) =>
        cache.GetOrCreateAsync(ByNameKey(name), () => SearchByNameCoreAsync(name));

    public Task<IEnumerable<ProductDto>> SearchByCategoryAsync(int categoryId) =>
        cache.GetOrCreateAsync(ByCategoryKey(categoryId), () => repository.GetByCategoryIdAsync(categoryId));

    public Task<IEnumerable<ProductDto>> SearchAsync(string? name, int? categoryId) =>
        cache.GetOrCreateAsync(ByNameAndCategoryKey(name, categoryId), () => SearchCoreAsync(name, categoryId));

    public void Invalidate() => cache.Invalidate();

    private async Task<IEnumerable<ProductDto>> SearchByNameCoreAsync(string name)
    {
        var all = await repository.GetAllAsync();
        return all.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IEnumerable<ProductDto>> SearchCoreAsync(string? name, int? categoryId)
    {
        var results = categoryId.HasValue
            ? await repository.GetByCategoryIdAsync(categoryId.Value)
            : await repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(name))
            results = results.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        return results;
    }
}
