namespace ProductCatalogManager.Domain.DTOs;

public sealed record ProductDto(
    int Id,
    string Name,
    string? Description,
    string SKU,
    decimal Price,
    int Quantity,
    int CategoryId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);