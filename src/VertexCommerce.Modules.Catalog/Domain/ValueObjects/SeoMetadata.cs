namespace VertexCommerce.Modules.Catalog.Domain.ValueObjects;

public readonly record struct SeoMetadata
{
    public Slug Slug { get; }
    public MetaTitle? MetaTitle { get; }
    public MetaDescription? MetaDescription { get; }
    public SeoKeywords? Keywords { get; }

    private SeoMetadata(Slug slug, MetaTitle? metaTitle, MetaDescription? metaDescription, SeoKeywords? keywords)
    {
        Slug = slug;
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        Keywords = keywords;
    }

    public static SeoMetadata Create(Slug slug, MetaTitle? metaTitle = null, MetaDescription? metaDescription = null, SeoKeywords? keywords = null)
    {
        return new SeoMetadata(slug, metaTitle, metaDescription, keywords);
    }
}
