using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Entities;

public sealed class ProductAttribute : Entity<Guid>
{
    public Guid ProductId { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? Type { get; private set; }

    private ProductAttribute()
    {
    }

    public static ProductAttribute Create(Guid productId, string key, string value, string? type = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Attribute key cannot be empty.", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Attribute value cannot be empty.", nameof(value));
        }

        return new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Key = key.Trim(),
            Value = value.Trim(),
            Type = type?.Trim()
        };
    }

    public void Update(string value, string? type = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Attribute value cannot be empty.", nameof(value));
        }

        Value = value.Trim();
        Type = type?.Trim();
        SetUpdatedAt();
    }
}
