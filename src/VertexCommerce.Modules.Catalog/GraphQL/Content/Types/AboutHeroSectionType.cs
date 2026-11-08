using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

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
