using Microsoft.EntityFrameworkCore;
using ProductCatalogManager.Domain.DTOs;

namespace ProductCatalogManager.Domain.Data;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<ProductDto> Products => Set<ProductDto>();
    public DbSet<CategoryDTO> Categories => Set<CategoryDTO>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductDto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.Name);
            entity.HasIndex(p => p.SKU).IsUnique();
            entity.HasIndex(p => p.CategoryId);
            entity.HasIndex(p => p.CreatedAt);
        });

        modelBuilder.Entity<CategoryDTO>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.Name);
            entity.HasIndex(c => c.ParentCategoryId);
        });
    }
}