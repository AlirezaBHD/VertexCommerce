using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Api.GraphQL.Catalog.Types;

public sealed class MediaType : ObjectType<ProductMediaReadModel>
{
    protected override void Configure(
        IObjectTypeDescriptor<ProductMediaReadModel> descriptor)
    {
        descriptor.Name("ProductMedia");

        descriptor.Field(m => m.Path).Type<NonNullType<StringType>>();
        descriptor.Field(m => m.AltText).Type<NonNullType<StringType>>();
        descriptor.Field(m => m.Type).Type<NonNullType<StringType>>();
        descriptor.Field(m => m.Order).Type<NonNullType<IntType>>();
    }
}
