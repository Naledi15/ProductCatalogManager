using Microsoft.AspNetCore.Mvc;
using ProductCatalogManager.API.Contracts.Requests.Products;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;
using ProductCatalogManager.Queries;

namespace ProductCatalogManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    IProductRepository products,
    IProductSearchEngine searchEngine) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool inStock = false,
        [FromQuery] string? sortBy = null)
    {
        var (items, totalCount) = (await products.GetAllAsync())
            .AsQueryable()
            .InCategory(categoryId)
            .MatchingSearch(search)
            .InPriceRange(minPrice, maxPrice)
            .InStockOnly(inStock)
            .SortBy(sortBy)
            .ToPage(page, pageSize);

        return Ok(new
        {
            items,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await products.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string? name = null,
        [FromQuery] int? categoryId = null)
    {
        var results = await searchEngine.SearchAsync(name, categoryId);
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
    {
        var now = DateTime.UtcNow;
        var product = await products.AddAsync(
            new ProductDto(0, request.Name, request.Description, request.Sku, request.Price, request.Quantity, request.CategoryId, now, now));
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequest request)
    {
        var existing = await products.GetByIdAsync(id);
        if (existing is null) return NotFound();

        var updated = await products.UpdateAsync(existing with
        {
            Name = request.Name,
            Description = request.Description,
            SKU = request.Sku,
            Price = request.Price,
            Quantity = request.Quantity,
            CategoryId = request.CategoryId,
            UpdatedAt = DateTime.UtcNow
        });
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (await products.GetByIdAsync(id) is null) return NotFound();
        await products.DeleteAsync(id);
        return NoContent();
    }
}
