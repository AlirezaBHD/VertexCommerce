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
