using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductCatalogManager.API.Contracts.Requests.Products;
using ProductCatalogManager.API.Data;
using ProductCatalogManager.API.Filters;
using ProductCatalogManager.API.Middleware;
using ProductCatalogManager.API.Serialization;
using ProductCatalogManager.Domain.Data;
using ProductCatalogManager.Domain.Interfaces;
using ProductCatalogManager.Domain.Repositories;
using ProductCatalogManager.Domain.Helpers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new CategoryTreeNodeConverter()));
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseInMemoryDatabase("ProductCatalogDb"));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
builder.Services.AddMemoryCache();
builder.Services.AddKeyedSingleton<CacheLayer>("products");
builder.Services.AddKeyedSingleton<CacheLayer>("categories");
builder.Services.AddScoped<IProductSearchEngine, ProductSearchEngine>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductRequestValidator>();

var app = builder.Build();
var requestAuditMiddleware =
    new RequestAuditMiddleware(app.Services.GetRequiredService<ILogger<RequestAuditMiddleware>>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Product Catalog Manager API")
               .WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.Use(async (context, next) =>
{
    await requestAuditMiddleware.InvokeAsync(context, next);
});

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    await CatalogData.SeedAsync(scope.ServiceProvider);
}

app.Run();