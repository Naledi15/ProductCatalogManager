using Microsoft.AspNetCore.Mvc;
using ProductCatalogManager.API.Contracts.Requests.Category;
using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryRepository categories) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories() =>
        Ok(await categories.GetAllAsync());

    [HttpGet("tree")]
    public async Task<IActionResult> GetCategoryTree() =>
        Ok(await categories.GetTreeAsync());

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
    {
        var category = await categories.AddAsync(
            new CategoryDTO(0, request.Name, request.Description, request.ParentCategoryId));
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
    }
}
