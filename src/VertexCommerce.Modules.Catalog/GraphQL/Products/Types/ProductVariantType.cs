using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Products.Types;

public sealed class ProductVariantType : ObjectType<ProductVariantReadModel>
{
    protected override void Configure(
        IObjectTypeDescriptor<ProductVariantReadModel> descriptor)
    {
        descriptor.Name("ProductVariant");
        descriptor.Field(v => v.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(p => p.Sku).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Price).Type<NonNullType<DecimalType>>();
        descriptor.Field(v => v.StockQuantity).Type<NonNullType<IntType>>();
        descriptor.Field(v => v.IsActive).Type<NonNullType<BooleanType>>();
        descriptor.Field(v => v.Attributes).Type<ListType<ProductAttributeType>>();
    }
}
