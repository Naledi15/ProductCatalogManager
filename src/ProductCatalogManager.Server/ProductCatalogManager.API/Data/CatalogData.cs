using ProductCatalogManager.Domain.DTOs;
using ProductCatalogManager.Domain.Interfaces;

namespace ProductCatalogManager.API.Data;

public static class CatalogData
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var categories = services.GetRequiredService<ICategoryRepository>();
        var products = services.GetRequiredService<IProductRepository>();

        if ((await categories.GetAllAsync()).Any() || (await products.GetAllAsync()).Any())
            return;

        // Repository<T> assigns IDs starting from 1 in insertion order
        // id 1
        await categories.AddAsync(new CategoryDTO(0, "Science", "Mathematics, physics, chemistry and biology textbooks", null));
        // id 2
        await categories.AddAsync(new CategoryDTO(0, "Computer Science", "Programming, algorithms and software engineering textbooks", 1));
        // id 3
        await categories.AddAsync(new CategoryDTO(0, "Humanities", "History, philosophy, literature and social sciences textbooks", null));
        // id 4
        await categories.AddAsync(new CategoryDTO(0, "Mathematics", "Pure and applied mathematics textbooks", 1));
        // id 5
        await categories.AddAsync(new CategoryDTO(0, "Business & Economics", "Finance, management and economics textbooks", null));
        // id 6
        await categories.AddAsync(new CategoryDTO(0, "Physics", "Classical, quantum and modern physics textbooks", 1));
        // id 7
        await categories.AddAsync(new CategoryDTO(0, "Chemistry", "General, organic and inorganic chemistry textbooks", 1));
        // id 8
        await categories.AddAsync(new CategoryDTO(0, "Finance", "Corporate finance, investments and financial markets textbooks", 5));
        // id 9
        await categories.AddAsync(new CategoryDTO(0, "Management", "Organizational behaviour, strategy and leadership textbooks", 5));

        await products.AddAsync(new ProductDto(0, "Introduction to Algorithms", "Comprehensive guide to algorithms and data structures", "TXT-001", 89.99m, 40, 2, new DateTime(2024, 1, 10), new DateTime(2024, 3, 15)));
        await products.AddAsync(new ProductDto(0, "Clean Code", "A handbook of agile software craftsmanship", "TXT-002", 49.99m, 60, 2, new DateTime(2024, 1, 12), new DateTime(2024, 2, 20)));
        await products.AddAsync(new ProductDto(0, "Calculus: Early Transcendentals", "Comprehensive calculus for science and engineering students", "TXT-003", 74.99m, 35, 1, new DateTime(2024, 1, 15), new DateTime(2024, 4, 1)));
        await products.AddAsync(new ProductDto(0, "A Brief History of Time", "Stephen Hawking's exploration of cosmology and the universe", "TXT-004", 19.99m, 80, 1, new DateTime(2024, 2, 1), new DateTime(2024, 3, 10)));
        await products.AddAsync(new ProductDto(0, "Sapiens: A Brief History of Humankind", "Yuval Noah Harari's account of human history", "TXT-005", 24.99m, 55, 3, new DateTime(2024, 2, 8), new DateTime(2024, 2, 8)));
        await products.AddAsync(new ProductDto(0, "Design Patterns", "Reusable solutions to common software design problems", "TXT-006", 59.99m, 45, 2, new DateTime(2024, 2, 14), new DateTime(2024, 2, 14)));
        await products.AddAsync(new ProductDto(0, "The Pragmatic Programmer", "Your journey to mastery in software development", "TXT-007", 54.99m, 50, 2, new DateTime(2024, 2, 20), new DateTime(2024, 3, 5)));
        await products.AddAsync(new ProductDto(0, "Organic Chemistry", "Comprehensive introduction to organic chemistry principles", "TXT-008", 94.99m, 30, 1, new DateTime(2024, 3, 1), new DateTime(2024, 3, 1)));
        await products.AddAsync(new ProductDto(0, "Linear Algebra and Its Applications", "Core concepts of linear algebra with real-world applications", "TXT-009", 79.99m, 38, 4, new DateTime(2024, 3, 5), new DateTime(2024, 3, 20)));
        await products.AddAsync(new ProductDto(0, "The Republic", "Plato's foundational work on justice, politics and philosophy", "TXT-010", 14.99m, 70, 3, new DateTime(2024, 3, 8), new DateTime(2024, 3, 8)));
        await products.AddAsync(new ProductDto(0, "Thinking, Fast and Slow", "Daniel Kahneman's exploration of the two systems of thought", "TXT-011", 22.99m, 65, 3, new DateTime(2024, 3, 12), new DateTime(2024, 3, 25)));
        await products.AddAsync(new ProductDto(0, "Principles of Economics", "Mankiw's essential introduction to economic theory", "TXT-012", 84.99m, 42, 5, new DateTime(2024, 3, 15), new DateTime(2024, 3, 15)));
        await products.AddAsync(new ProductDto(0, "Operating System Concepts", "In-depth coverage of OS design and implementation", "TXT-013", 99.99m, 28, 2, new DateTime(2024, 3, 18), new DateTime(2024, 4, 2)));
        await products.AddAsync(new ProductDto(0, "University Physics", "Comprehensive physics for science and engineering", "TXT-014", 89.99m, 32, 6, new DateTime(2024, 3, 20), new DateTime(2024, 3, 20)));
        await products.AddAsync(new ProductDto(0, "Introduction to Quantum Mechanics", "Griffiths' accessible guide to quantum theory", "TXT-015", 79.99m, 25, 6, new DateTime(2024, 3, 22), new DateTime(2024, 4, 1)));
        await products.AddAsync(new ProductDto(0, "Chemistry: The Central Science", "Broad introduction to general chemistry principles", "TXT-016", 94.99m, 30, 7, new DateTime(2024, 3, 25), new DateTime(2024, 3, 25)));
        await products.AddAsync(new ProductDto(0, "Organic Chemistry", "Comprehensive introduction to organic chemistry principles", "TXT-017", 99.99m, 22, 7, new DateTime(2024, 3, 27), new DateTime(2024, 4, 3)));
        await products.AddAsync(new ProductDto(0, "Principles of Corporate Finance", "Brealey & Myers' definitive guide to corporate finance", "TXT-018", 109.99m, 20, 8, new DateTime(2024, 4, 1), new DateTime(2024, 4, 1)));
        await products.AddAsync(new ProductDto(0, "The Intelligent Investor", "Benjamin Graham's classic guide to value investing", "TXT-019", 22.99m, 75, 8, new DateTime(2024, 4, 3), new DateTime(2024, 4, 3)));
        await products.AddAsync(new ProductDto(0, "Competitive Strategy", "Michael Porter's framework for industry analysis", "TXT-020", 49.99m, 35, 9, new DateTime(2024, 4, 5), new DateTime(2024, 4, 5)));
        await products.AddAsync(new ProductDto(0, "Organizational Behaviour", "Understanding people and groups in the workplace", "TXT-021", 74.99m, 40, 9, new DateTime(2024, 4, 7), new DateTime(2024, 4, 7)));
    }
}
