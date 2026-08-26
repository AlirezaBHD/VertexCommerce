using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutType : ObjectType<AboutDocument>
{
    protected override void Configure(IObjectTypeDescriptor<AboutDocument> descriptor)
    {
        descriptor.Name("AboutContent");

        descriptor.Field(a => a.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(a => a.Hero).Type<NonNullType<AboutHeroSectionType>>();
        descriptor.Field(a => a.Commitments).Type<NonNullType<AboutCommitmentsSectionType>>();
        descriptor.Field(a => a.Quality).Type<NonNullType<AboutQualitySectionType>>();
        descriptor.Field(a => a.Process).Type<NonNullType<AboutProcessSectionType>>();
        descriptor.Field(a => a.Story).Type<NonNullType<AboutStorySectionType>>();
        descriptor.Field(a => a.Cta).Type<NonNullType<AboutCtaSectionType>>();
        descriptor.Field(a => a.UpdatedAt).Type<NonNullType<DateTimeType>>();
    }
}

public sealed class AboutHeroSectionType : ObjectType<AboutHeroSection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutHeroSection> descriptor)
    {
        descriptor.Name("AboutHeroSection");
        descriptor.Field(h => h.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.Title).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.ButtonText).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.ImagePath).Type<StringType>();
        descriptor.Field(h => h.ShowCat).Type<NonNullType<BooleanType>>();
        descriptor.Field(h => h.CatImagePath).Type<StringType>();
    }
}

public sealed class AboutCommitmentsSectionType : ObjectType<AboutCommitmentsSection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutCommitmentsSection> descriptor)
    {
        descriptor.Name("AboutCommitmentsSection");
        descriptor.Field(c => c.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Title).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Items).Type<NonNullType<ListType<NonNullType<AboutCommitmentItemType>>>>();
    }
}

public sealed class AboutCommitmentItemType : ObjectType<AboutCommitmentItem>
{
    protected override void Configure(IObjectTypeDescriptor<AboutCommitmentItem> descriptor)
    {
        descriptor.Name("AboutCommitmentItem");
        descriptor.Field(i => i.Title).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Description).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Icon).Type<NonNullType<StringType>>();
    }
}

public sealed class AboutQualitySectionType : ObjectType<AboutQualitySection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutQualitySection> descriptor)
    {
        descriptor.Name("AboutQualitySection");
        descriptor.Field(q => q.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(q => q.Title).Type<NonNullType<StringType>>();
        descriptor.Field(q => q.Paragraphs).Type<NonNullType<ListType<NonNullType<StringType>>>>();
        descriptor.Field(q => q.Features).Type<NonNullType<ListType<NonNullType<AboutQualityFeatureItemType>>>>();
        descriptor.Field(q => q.ImagePath).Type<StringType>();
        descriptor.Field(q => q.ImageBadgeTitle).Type<StringType>();
        descriptor.Field(q => q.ImageBadgeSubtitle).Type<StringType>();
    }
}

public sealed class AboutQualityFeatureItemType : ObjectType<AboutQualityFeatureItem>
{
    protected override void Configure(IObjectTypeDescriptor<AboutQualityFeatureItem> descriptor)
    {
        descriptor.Name("AboutQualityFeatureItem");
        descriptor.Field(f => f.Title).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.Description).Type<NonNullType<StringType>>();
    }
}

public sealed class AboutProcessSectionType : ObjectType<AboutProcessSection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutProcessSection> descriptor)
    {
        descriptor.Name("AboutProcessSection");
        descriptor.Field(p => p.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Title).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Steps).Type<NonNullType<ListType<NonNullType<AboutProcessStepItemType>>>>();
    }
}

public sealed class AboutProcessStepItemType : ObjectType<AboutProcessStepItem>
{
    protected override void Configure(IObjectTypeDescriptor<AboutProcessStepItem> descriptor)
    {
        descriptor.Name("AboutProcessStepItem");
        descriptor.Field(s => s.Title).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Description).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Icon).Type<NonNullType<StringType>>();
    }
}

public sealed class AboutStorySectionType : ObjectType<AboutStorySection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutStorySection> descriptor)
    {
        descriptor.Name("AboutStorySection");
        descriptor.Field(s => s.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Title).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Paragraphs).Type<NonNullType<ListType<NonNullType<StringType>>>>();
        descriptor.Field(s => s.ImagePath).Type<StringType>();
        descriptor.Field(s => s.ImageBadge).Type<StringType>();
        descriptor.Field(s => s.SupportText).Type<StringType>();
    }
}

public sealed class AboutCtaSectionType : ObjectType<AboutCtaSection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutCtaSection> descriptor)
    {
        descriptor.Name("AboutCtaSection");
        descriptor.Field(c => c.Title).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.ButtonText).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.ButtonLink).Type<NonNullType<StringType>>();
    }
}
