using ProductCatalogManager.Domain.DTOs;

namespace ProductCatalogManager.Domain.Interfaces;

public interface ICategoryRepository : IRepository<CategoryDTO>
{
    Task<IEnumerable<CategoryDTO>> GetByParentIdAsync(int? parentCategoryId);
}
