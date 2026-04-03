using Microsoft.AspNetCore.Mvc;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null)
    {
        var query = (await products.GetAllAsync()).AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

        var totalCount = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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

public record ProductRequest(
    string Name,
    string Description,
    string Sku,
    decimal Price,
    int Quantity,
    int CategoryId);
