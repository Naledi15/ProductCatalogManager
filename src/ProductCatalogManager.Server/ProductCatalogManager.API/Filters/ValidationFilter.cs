using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProductCatalogManager.API.Filters;

public sealed class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var (_, value) in context.ActionArguments)
        {
            if (value is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(value.GetType());

            if (serviceProvider.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(value);
            var result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(errors));
                return;
            }
        }

        await next();
    }
}
