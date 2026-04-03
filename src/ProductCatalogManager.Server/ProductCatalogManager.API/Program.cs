using ProductCatalogManager.API.Data;
using ProductCatalogManager.Domain.Interfaces;
using ProductCatalogManager.Domain.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();

var app = builder.Build();

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

app.UseHttpsRedirection();
app.MapControllers();

await CatalogData.SeedAsync(app.Services);

app.Run();