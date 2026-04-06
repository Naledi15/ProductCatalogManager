using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public sealed class ProductSearchEngine(IProductRepository repository) : IProductSearchEngine
{
    public async Task<IEnumerable<ProductDto>> SearchByNameAsync(string name)
    {
        var all = await repository.GetAllAsync();
        return all.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<ProductDto>> SearchByCategoryAsync(int categoryId) =>
        await repository.GetByCategoryIdAsync(categoryId);

    public async Task<IEnumerable<ProductDto>> SearchAsync(string? name, int? categoryId)
    {
        var results = categoryId.HasValue
            ? await repository.GetByCategoryIdAsync(categoryId.Value)
            : await repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(name))
            results = results.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        return results;
    }

    // No-op: caching lives in the SearchResultCaching decorator, not here.
    public void Invalidate() { }
}
