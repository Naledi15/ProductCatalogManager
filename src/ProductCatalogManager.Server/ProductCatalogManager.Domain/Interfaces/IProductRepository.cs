using ProductCatalogManager.Domain.DTOs;

namespace ProductCatalogManager.Domain.Interfaces;

public interface IProductRepository : IRepository<ProductDto>
{
    Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId);
    Task<ProductDto?> GetBySkuAsync(string sku);
}
