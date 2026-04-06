using ProductCatalogManager.Domain.DTOs;

namespace ProductCatalogManager.Domain.Interfaces;

public interface IProductSearchEngine
{    Task<IEnumerable<ProductDto>> SearchByNameandCategoryAsync(string? name, int? categoryId);
    void Invalidate();
}
