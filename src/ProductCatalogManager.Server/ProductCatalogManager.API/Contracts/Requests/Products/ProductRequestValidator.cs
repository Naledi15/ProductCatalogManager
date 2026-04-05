using FluentValidation;

namespace ProductCatalogManager.API.Contracts.Requests.Products;

public sealed class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid CategoryId is required.");
    }
}
