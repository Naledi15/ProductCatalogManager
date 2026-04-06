namespace ProductCatalogManager.API.Contracts.Responses;

public sealed record ProductResponse(
    int Id,
    string Name,
    string? Description,
    string SKU,
    decimal Price,
    int Quantity,
    int CategoryId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
