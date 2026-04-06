namespace ProductCatalogManager.Queries.Models;

public sealed record ProductParameters(
    int Page = 1,
    int PageSize = 10,
    int? CategoryId = null,
    string? Search = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool InStock = false,
    string? SortBy = null);
