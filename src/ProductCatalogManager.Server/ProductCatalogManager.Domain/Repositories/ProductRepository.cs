using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public class ProductRepository : Repository<ProductDto>, IProductRepository
{
    protected override int GetId(ProductDto entity) => entity.Id;
    protected override ProductDto WithId(ProductDto entity, int id) => entity with { Id = id };

    public async Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId) =>
        (await GetAllAsync()).Where(p => p.CategoryId == categoryId);

    public async Task<ProductDto?> GetBySkuAsync(string sku) =>
        (await GetAllAsync()).FirstOrDefault(p => p.SKU == sku);
}
