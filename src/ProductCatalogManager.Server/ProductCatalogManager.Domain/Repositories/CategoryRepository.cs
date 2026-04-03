using Microsoft.EntityFrameworkCore;
using ProductCatalogManager.Domain.Data;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public class CategoryRepository(CatalogDbContext dbContext) : ICategoryRepository
{
    public async Task<CategoryDTO?> GetByIdAsync(int id) =>
        await dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<CategoryDTO>> GetAllAsync() =>
        await dbContext.Categories.AsNoTracking().ToListAsync();

    public async Task<CategoryDTO> AddAsync(CategoryDTO entity)
    {
        var lastId = await dbContext.Categories
            .OrderByDescending(c => c.Id)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();
        var nextId = lastId + 1;
        var newEntity = entity with { Id = nextId };
        dbContext.Categories.Add(newEntity);
        await dbContext.SaveChangesAsync();
        return newEntity;
    }

    public async Task<CategoryDTO> UpdateAsync(CategoryDTO entity)
    {
        var exists = await dbContext.Categories.AnyAsync(c => c.Id == entity.Id);
        if (!exists)
            throw new KeyNotFoundException($"Entity with id {entity.Id} not found.");

        dbContext.Categories.Update(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (existing is null)
            return;

        dbContext.Categories.Remove(existing);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryDTO>> GetByParentIdAsync(int? parentCategoryId) =>
        await dbContext.Categories.AsNoTracking().Where(c => c.ParentCategoryId == parentCategoryId).ToListAsync();
}
