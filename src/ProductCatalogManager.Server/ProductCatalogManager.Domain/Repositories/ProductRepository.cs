using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public class ProductRepository : Repository<ProductDto>, IProductRepository
{
    protected override int GetId(ProductDto entity) => entity.Id;
    protected override ProductDto WithId(ProductDto entity, int id) => entity with { Id = id };

    public Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId) =>
        GetAllAsync().ContinueWith(t =>
            t.Result.Where(p => p.CategoryId == categoryId));

    public Task<ProductDto?> GetBySkuAsync(string sku) =>
        GetAllAsync().ContinueWith(t =>
            t.Result.FirstOrDefault(p => p.SKU == sku));
}
