using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Products.Types;

public sealed class ProductAttributeType : ObjectType<ProductAttributeReadModel>
{
    protected override void Configure(
        IObjectTypeDescriptor<ProductAttributeReadModel> descriptor)
    {
        descriptor.Name("ProductAttribute");
        descriptor.Field(p => p.AttributeCode).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.OptionCode).Type<NonNullType<StringType>>();
    }
}