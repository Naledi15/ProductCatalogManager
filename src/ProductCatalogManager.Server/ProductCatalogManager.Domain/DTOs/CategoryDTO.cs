namespace ProductCatalogManager.Domain.DTOs;

public sealed record CategoryDTO(
    int Id,
    string Name,
    string? Description,
    int? ParentCategoryId
);

public sealed record CategoryTreeNode(
    int Id,
    string Name,
    string? Description,
    int? ParentCategoryId,
    List<CategoryTreeNode> Children
);
