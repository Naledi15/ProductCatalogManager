using Microsoft.EntityFrameworkCore;
using ProductCatalogManager.Domain.DTOs;

namespace ProductCatalogManager.Domain.Data;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<ProductDto> Products => Set<ProductDto>();
    public DbSet<CategoryDTO> Categories => Set<CategoryDTO>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductDto>().HasKey(p => p.Id);
        modelBuilder.Entity<CategoryDTO>().HasKey(c => c.Id);
    }
}