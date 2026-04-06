using ProductCatalogManager.Domain.DTOs;

namespace ProductCatalogManager.Domain.Interfaces;

public interface IProductSearchEngine
{
    Task<IEnumerable<ProductDto>> SearchByNameAsync(string name);
    Task<IEnumerable<ProductDto>> SearchByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> SearchAsync(string? name, int? categoryId);
    void Invalidate();
}
