using ProductCatalogManager.Domain.DTOs;

namespace ProductCatalogManager.Queries;

public static class ProductQueryExtensions
{
    /// <summary>Filters products by category when a value is provided.</summary>
    public static IQueryable<ProductDto> InCategory(this IQueryable<ProductDto> query, int? categoryId) =>
        categoryId is int id ? query.Where(p => p.CategoryId == id) : query;

    /// <summary>Filters products whose name or SKU contains the search term (case-insensitive).</summary>
    public static IQueryable<ProductDto> MatchingSearch(this IQueryable<ProductDto> query, string? search) =>
        string.IsNullOrWhiteSpace(search)
            ? query
            : query.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.SKU.Contains(search, StringComparison.OrdinalIgnoreCase));

    /// <summary>Filters products within an inclusive price range.</summary>
    public static IQueryable<ProductDto> InPriceRange(this IQueryable<ProductDto> query, decimal? min, decimal? max)
    {
        if (min is decimal minVal) query = query.Where(p => p.Price >= minVal);
        if (max is decimal maxVal) query = query.Where(p => p.Price <= maxVal);
        return query;
    }

    /// <summary>Filters to only in-stock products (Quantity > 0).</summary>
    public static IQueryable<ProductDto> InStockOnly(this IQueryable<ProductDto> query, bool inStockOnly) =>
        inStockOnly ? query.Where(p => p.Quantity > 0) : query;

    /// <summary>Sorts by a named field; defaults to Id ascending.</summary>
    public static IQueryable<ProductDto> SortBy(this IQueryable<ProductDto> query, string? sortBy) =>
        sortBy?.ToLowerInvariant() switch
        {
            "name"       => query.OrderBy(p => p.Name),
            "name_desc"  => query.OrderByDescending(p => p.Name),
            "price"      => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "quantity"   => query.OrderBy(p => p.Quantity),
            _            => query.OrderBy(p => p.Id)
        };

    /// <summary>Returns a single page of results along with total count metadata.</summary>
    public static (List<ProductDto> Items, int TotalCount) ToPage(
        this IQueryable<ProductDto> query, int page, int pageSize)
    {
        var totalCount = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (items, totalCount);
    }
}
