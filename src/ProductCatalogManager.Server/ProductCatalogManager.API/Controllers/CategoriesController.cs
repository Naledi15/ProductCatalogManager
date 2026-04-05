using Microsoft.AspNetCore.Mvc;
using ProductCatalogManager.API.Contracts.Category;
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
    public async Task<IActionResult> GetCategoryTree()
    {
        var all = (await categories.GetAllAsync()).ToList();
        return Ok(BuildTree(all, null));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
    {
        var category = await categories.AddAsync(
            new CategoryDTO(0, request.Name, request.Description, request.ParentCategoryId));
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
    }

    private static List<CategoryTreeNode> BuildTree(List<CategoryDTO> all, int? parentId) =>
        all
            .Where(c => c.ParentCategoryId == parentId)
            .Select(c => new CategoryTreeNode(c.Id, c.Name, c.Description, c.ParentCategoryId, BuildTree(all, c.Id)))
            .ToList();
}

public record CategoryTreeNode(
    int Id,
    string Name,
    string Description,
    int? ParentCategoryId,
    List<CategoryTreeNode> Children);
