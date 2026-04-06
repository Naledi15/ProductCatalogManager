namespace ProductCatalogManager.Utilities.SearchEngine;

/// <summary>Pairs an item with its computed relevance score.</summary>
public sealed record SearchResult<T>(T Item, double Score);