using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;

public sealed class ProductMedia : ValueObject
{
    public string Path { get; private set; } = string.Empty;
    public MediaType Type { get; private set; }
    public int SortOrder { get; private set; }
    public string? AltText { get; private set; } 
    public string? AssociatedAttributeCode { get; private set; }
    public string? AssociatedOptionCode { get; private set; }

    // public ProductAttribute? AssociatedAttribute => 
    //     !string.IsNullOrEmpty(AssociatedAttributeCode) && !string.IsNullOrEmpty(AssociatedOptionCode)
    //         ? ProductAttribute.Create(AssociatedAttributeCode, AssociatedOptionCode)
    //         : null;
    
    private ProductMedia() { }

    public static ProductMedia Create(string path, MediaType type, int order = 0, string? altText = null, string? associatedAttributeCode = null, string? associatedOptionCode = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Media path cannot be empty.", nameof(path));

        return new ProductMedia
        {
            Path = path.Trim(),
            Type = type,
            SortOrder = order,
            AltText = altText?.Trim(),
            AssociatedAttributeCode = associatedAttributeCode?.Trim(),
            AssociatedOptionCode =  associatedOptionCode?.Trim()
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Path;
        yield return Type;
    }
}

public enum MediaType
{
    Image = 1,
    Video = 2
}
