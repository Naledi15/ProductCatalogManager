namespace ProductCatalogManager.Domain.DTOs;

public static class ProductComparers
{
    public static readonly IComparer<ProductDto> ById = Comparer<ProductDto>.Create((x, y) => x.Id.CompareTo(y.Id));
    public static readonly IComparer<ProductDto> ByIdDesc = Comparer<ProductDto>.Create((x, y) => y.Id.CompareTo(x.Id));

    public static readonly IComparer<ProductDto> ByName = Comparer<ProductDto>.Create((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
    public static readonly IComparer<ProductDto> ByNameDesc = Comparer<ProductDto>.Create((x, y) => string.Compare(y.Name, x.Name, StringComparison.OrdinalIgnoreCase));

    public static readonly IComparer<ProductDto> BySku = Comparer<ProductDto>.Create((x, y) => string.Compare(x.SKU, y.SKU, StringComparison.OrdinalIgnoreCase));
    public static readonly IComparer<ProductDto> BySkuDesc = Comparer<ProductDto>.Create((x, y) => string.Compare(y.SKU, x.SKU, StringComparison.OrdinalIgnoreCase));

    public static readonly IComparer<ProductDto> ByPrice = Comparer<ProductDto>.Create((x, y) => x.Price.CompareTo(y.Price));
    public static readonly IComparer<ProductDto> ByPriceDesc = Comparer<ProductDto>.Create((x, y) => y.Price.CompareTo(x.Price));

    public static readonly IComparer<ProductDto> ByQuantity = Comparer<ProductDto>.Create((x, y) => x.Quantity.CompareTo(y.Quantity));
    public static readonly IComparer<ProductDto> ByQuantityDesc = Comparer<ProductDto>.Create((x, y) => y.Quantity.CompareTo(x.Quantity));

    public static readonly IComparer<ProductDto> ByCategory = Comparer<ProductDto>.Create((x, y) => x.CategoryId.CompareTo(y.CategoryId));
    public static readonly IComparer<ProductDto> ByCategoryDesc = Comparer<ProductDto>.Create((x, y) => y.CategoryId.CompareTo(x.CategoryId));

    public static readonly IComparer<ProductDto> ByCreated = Comparer<ProductDto>.Create((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
    public static readonly IComparer<ProductDto> ByCreatedDesc = Comparer<ProductDto>.Create((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));

    public static readonly IComparer<ProductDto> ByUpdated = Comparer<ProductDto>.Create((x, y) => x.UpdatedAt.CompareTo(y.UpdatedAt));
    public static readonly IComparer<ProductDto> ByUpdatedDesc = Comparer<ProductDto>.Create((x, y) => y.UpdatedAt.CompareTo(x.UpdatedAt));

    public static void SortBy(this List<ProductDto> list, string? sortBy)
    {
        var comparer = (sortBy?.ToLowerInvariant()) switch
        {
            "id"            => ProductComparers.ById,
            "id_desc"       => ProductComparers.ByIdDesc,
            "name"          => ProductComparers.ByName,
            "name_desc"     => ProductComparers.ByNameDesc,
            "sku"           => ProductComparers.BySku,
            "sku_desc"      => ProductComparers.BySkuDesc,
            "price"         => ProductComparers.ByPrice,
            "price_desc"    => ProductComparers.ByPriceDesc,
            "quantity"      => ProductComparers.ByQuantity,
            "quantity_desc" => ProductComparers.ByQuantityDesc,
            "category"      => ProductComparers.ByCategory,
            "category_desc" => ProductComparers.ByCategoryDesc,
            "created"       => ProductComparers.ByCreated,
            "created_desc"  => ProductComparers.ByCreatedDesc,
            "updated"       => ProductComparers.ByUpdated,
            "updated_desc"  => ProductComparers.ByUpdatedDesc,
            _               => ProductComparers.ById
        };

        list.Sort(comparer);
    }
}
