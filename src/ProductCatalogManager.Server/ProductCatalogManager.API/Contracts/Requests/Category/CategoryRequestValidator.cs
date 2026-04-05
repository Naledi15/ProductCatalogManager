using FluentValidation;

namespace ProductCatalogManager.API.Contracts.Requests.Category;

public sealed class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0).When(x => x.ParentCategoryId.HasValue)
            .WithMessage("ParentCategoryId must be a positive integer.");
    }
}
