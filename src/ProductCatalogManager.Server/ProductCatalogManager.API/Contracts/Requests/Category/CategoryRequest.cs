namespace ProductCatalogManager.API.Contracts.Requests.Category;

public sealed record CategoryRequest(
    string Name,
    string Description,
    int? ParentCategoryId);
