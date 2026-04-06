namespace ProductCatalogManager.Utilities.SearchEngine;

public sealed class SearchField<T>
{
    public string Name { get; }
    public Func<T, string?> Extractor { get; }
    public double Weight { get; }

    public SearchField(string name, Func<T, string?> extractor, double weight = 1.0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name is required.", nameof(name));
        ArgumentNullException.ThrowIfNull(extractor);
        if (weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be positive.");

        Name = name;
        Extractor = extractor;
        Weight = weight;
    }
}