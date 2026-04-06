using System.Text.Json;
using System.Text.Json.Serialization;
using ProductCatalogManager.Domain.DTOs;

namespace ProductCatalogManager.API.Serialization;

/// <summary>
/// Custom JSON converter for <see cref="CategoryTreeNode"/>.
/// Differences from default serialisation:
///   - Uses snake_case property names
///   - Omits "parent_category_id" when null
///   - Omits "children" when the list is empty (leaf nodes)
/// </summary>
public sealed class CategoryTreeNodeConverter : JsonConverter<CategoryTreeNode>
{
    public override CategoryTreeNode Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var id = root.GetProperty("id").GetInt32();
        var name = root.GetProperty("name").GetString()!;
        var description = root.TryGetProperty("description", out var descEl)
            ? descEl.GetString()
            : null;

        int? parentCategoryId = root.TryGetProperty("parent_category_id", out var parentEl)
            ? parentEl.GetInt32()
            : null;

        var children = new List<CategoryTreeNode>();
        if (root.TryGetProperty("children", out var childrenEl))
            foreach (var child in childrenEl.EnumerateArray())
                children.Add(JsonSerializer.Deserialize<CategoryTreeNode>(child.GetRawText(), options)!);

        return new CategoryTreeNode(id, name, description, parentCategoryId, children);
    }

    public override void Write(
        Utf8JsonWriter writer, CategoryTreeNode node, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("id", node.Id);
        writer.WriteString("name", node.Name);
        // Omit description when null
        if (node.Description is not null)
            writer.WriteString("description", node.Description);

        // Omit parent_category_id for root nodes (null)
        if (node.ParentCategoryId.HasValue)
            writer.WriteNumber("parent_category_id", node.ParentCategoryId.Value);

        // Omit children array on leaf nodes
        if (node.Children.Count > 0)
        {
            writer.WritePropertyName("children");
            writer.WriteStartArray();
            foreach (var child in node.Children)
                JsonSerializer.Serialize(writer, child, options);
            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }
}
