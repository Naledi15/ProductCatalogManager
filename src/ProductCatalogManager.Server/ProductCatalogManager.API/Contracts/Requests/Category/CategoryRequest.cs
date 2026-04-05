namespace ProductCatalogManager.API.Contracts.Category;

public sealed record CategoryRequest(
    string Name,
    string Description,
    int? ParentCategoryId);
