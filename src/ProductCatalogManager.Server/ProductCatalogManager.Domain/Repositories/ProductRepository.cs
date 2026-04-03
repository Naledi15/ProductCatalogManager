using Microsoft.EntityFrameworkCore;
using ProductCatalogManager.Domain.Data;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.Domain.Repositories;

public class ProductRepository(CatalogDbContext dbContext) : IProductRepository
{
    public async Task<ProductDto?> GetByIdAsync(int id) =>
        await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<ProductDto>> GetAllAsync() =>
        await dbContext.Products.AsNoTracking().ToListAsync();

    public async Task<ProductDto> AddAsync(ProductDto entity)
    {
        var lastId = await dbContext.Products
            .OrderByDescending(p => p.Id)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();
        var nextId = lastId + 1;
        var newEntity = entity with { Id = nextId };
        dbContext.Products.Add(newEntity);
        await dbContext.SaveChangesAsync();
        return newEntity;
    }

    public async Task<ProductDto> UpdateAsync(ProductDto entity)
    {
        var exists = await dbContext.Products.AnyAsync(p => p.Id == entity.Id);
        if (!exists)
            throw new KeyNotFoundException($"Entity with id {entity.Id} not found.");

        dbContext.Products.Update(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (existing is null)
            return;

        dbContext.Products.Remove(existing);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId) =>
        await dbContext.Products.AsNoTracking().Where(p => p.CategoryId == categoryId).ToListAsync();

    public async Task<ProductDto?> GetBySkuAsync(string sku) =>
        await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.SKU == sku);
}
