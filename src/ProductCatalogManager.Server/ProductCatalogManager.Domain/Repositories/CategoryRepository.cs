using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public class CategoryRepository : Repository<CategoryDTO>, ICategoryRepository
{
    protected override int GetId(CategoryDTO entity) => entity.Id;
    protected override CategoryDTO WithId(CategoryDTO entity, int id) => entity with { Id = id };

    public async Task<IEnumerable<CategoryDTO>> GetByParentIdAsync(int? parentCategoryId) =>
        (await GetAllAsync()).Where(c => c.ParentCategoryId == parentCategoryId);

    public async Task<List<CategoryTreeNode>> GetTreeAsync()
    {
        var all = (await GetAllAsync()).ToList();
        return BuildTree(all, null);
    }

    private static List<CategoryTreeNode> BuildTree(List<CategoryDTO> all, int? parentId) =>
        all
            .Where(c => c.ParentCategoryId == parentId)
            .Select(c => new CategoryTreeNode(c.Id, c.Name, c.Description, c.ParentCategoryId, BuildTree(all, c.Id)))
            .ToList();
}
