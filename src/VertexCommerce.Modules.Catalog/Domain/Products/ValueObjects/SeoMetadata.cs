using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;

public sealed class SeoMetadata : ValueObject
{
    public string Slug { get; private set; } = string.Empty;
    public string MetaTitle { get; private set; } = string.Empty;
    public string MetaDescription { get; private set; } = string.Empty;
    public string? Keywords { get; private set; }

    private SeoMetadata() { }

    public static SeoMetadata Create(
        string slug,
        string metaTitle,
        string metaDescription,
        string? keywords = null)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug cannot be empty.");

        if (string.IsNullOrWhiteSpace(metaTitle))
            throw new ArgumentException("Meta title cannot be empty.");

        if (string.IsNullOrWhiteSpace(metaDescription))
            throw new ArgumentException("Meta description cannot be empty.");

        var normalizedSlug = NormalizeSlug(slug);

        if (metaTitle.Length > 60)
            throw new ArgumentException("Meta title should not exceed 60 characters.");

        if (metaDescription.Length > 160)
            throw new ArgumentException("Meta description should not exceed 160 characters.");

        return new SeoMetadata
        {
            Slug = normalizedSlug,
            MetaTitle = metaTitle,
            MetaDescription = metaDescription,
            Keywords = keywords
        };
    }

    private static string NormalizeSlug(string slug)
    {
        return slug
            .ToLowerInvariant()
            .Trim()
            .Replace(" ", "-")
            .Replace("_", "-");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Slug;
        yield return MetaTitle;
        yield return MetaDescription;
        yield return Keywords;
    }
}