using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalogManager.API.Contracts.Requests.Products;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Helpers;
using ProductCatalogManager.Domain.Interfaces;
using ProductCatalogManager.Queries;

namespace ProductCatalogManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    IProductRepository products,
    IProductSearchEngine searchEngine,
    [FromKeyedServices("products")] CacheLayer productCache) : ControllerBase
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
        // Cache the filtered (unsorted, unpaged) list; sort + page after retrieval.
        var filterKey = $"products:cat={categoryId}:s={search}:min={minPrice}:max={maxPrice}:stock={inStock}";
        var filtered = await productCache.GetOrCreateAsync(filterKey, async () =>
        {
            var all = await products.GetAllAsync();
            return all.AsQueryable()
                .InCategory(categoryId)
                .MatchingSearch(search)
                .InPriceRange(minPrice, maxPrice)
                .InStockOnly(inStock)
                .ToList();
        });

        var sorted = filtered.ToList();
        ProductComparers.SortBy(sorted, sortBy);

        var totalCount = sorted.Count;
        var items = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
    public async Task<IActionResult> GetProduct()
    {
        var routeId = RouteData.Values["id"]?.ToString();

        if (string.IsNullOrEmpty(routeId) || !int.TryParse(routeId, out int id))
        {
            return BadRequest("A valid integer ID must be provided in the URL.");
        }

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
        searchEngine.Invalidate();
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
        searchEngine.Invalidate();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (await products.GetByIdAsync(id) is null) return NotFound();
        await products.DeleteAsync(id);
        searchEngine.Invalidate();
        return NoContent();
    }
}
