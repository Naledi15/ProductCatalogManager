using Microsoft.Extensions.DependencyInjection;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Helpers;
using ProductCatalogManager.Utilities.SearchEngine;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public sealed class ProductSearchEngine(
    IProductRepository repository,
    [FromKeyedServices("products")] CacheLayer cache) : IProductSearchEngine
{
    private static readonly SearchEngineUtility<ProductDto> Utility = new(
    [
        new SearchField<ProductDto>("Name",        p => p.Name,        weight: 2.0),
        new SearchField<ProductDto>("SKU",         p => p.SKU,         weight: 1.5),
        new SearchField<ProductDto>("Description", p => p.Description, weight: 1.0),
    ]);

    private static string CacheKey(string? name, int? categoryId) => $"search:name:{name}:cat:{categoryId}";

    public Task<IEnumerable<ProductDto>> SearchByNameandCategoryAsync(string? name, int? categoryId) =>
        cache.GetOrCreateAsync(CacheKey(name, categoryId), () => SearchCoreAsync(name, categoryId));

    public void Invalidate() => cache.Invalidate();

    private async Task<IEnumerable<ProductDto>> SearchCoreAsync(string? name, int? categoryId)
    {
        var pool = categoryId.HasValue
            ? await repository.GetByCategoryIdAsync(categoryId.Value)
            : await repository.GetAllAsync();

        if (string.IsNullOrWhiteSpace(name))
            return pool.ToList();

        return Utility.Search(pool, name)
                      .Select(r => r.Item)
                      .ToList();
    }
}

