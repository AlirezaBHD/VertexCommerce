using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Products.Types;

public sealed class ProductMediaType : ObjectType<ProductMediaReadModel>
{
    protected override void Configure(
        IObjectTypeDescriptor<ProductMediaReadModel> descriptor)
    {
        descriptor.Name("ProductMedia");

        descriptor.Field(m => m.Path).Type<NonNullType<StringType>>();
        descriptor.Field(m => m.Type).Type<NonNullType<StringType>>();
        descriptor.Field(m => m.SortOrder).Type<NonNullType<IntType>>();
        descriptor.Field(m => m.AltText).Type<StringType>();
        descriptor.Field(m => m.AssociatedAttributeCode).Type<StringType>();
        descriptor.Field(m => m.AssociatedOptionCode).Type<StringType>();
    }
}
