namespace ProductCatalogManager.API.Contracts.Requests.Products;

public sealed record ProductRequest(
    string Name,
    string Description,
    string Sku,
    decimal Price,
    int Quantity,
    int CategoryId);
