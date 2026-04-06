using FluentValidation;

namespace ProductCatalogManager.Queries.Models;

public sealed class ProductParametersValidator : AbstractValidator<ProductParameters>
{
    private static readonly HashSet<string> ValidSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "id", "id_desc",
        "name", "name_desc",
        "sku", "sku_desc",
        "price", "price_desc",
        "quantity", "quantity_desc",
        "category", "category_desc",
        "created", "created_desc",
        "updated", "updated_desc"
    };

    public ProductParametersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("MinPrice must be non-negative.")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage("MaxPrice must be non-negative.")
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x)
            .Must(x => x.MinPrice is null || x.MaxPrice is null || x.MinPrice <= x.MaxPrice)
            .WithName("PriceRange")
            .WithMessage("MinPrice must not exceed MaxPrice.");

        RuleFor(x => x.SortBy)
            .Must(s => s is null || ValidSortFields.Contains(s))
            .WithMessage($"SortBy must be one of: {string.Join(", ", ValidSortFields)}.");
    }
}
