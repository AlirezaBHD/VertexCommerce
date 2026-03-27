using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Api.GraphQL.Catalog.Types;

public sealed class VariantType : ObjectType<ProductVariantReadModel>
{
    protected override void Configure(
        IObjectTypeDescriptor<ProductVariantReadModel> descriptor)
    {
        descriptor.Name("ProductVariant");

        descriptor.Field(v => v.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(p => p.Sku).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Price).Type<NonNullType<DecimalType>>();
        descriptor.Field(v => v.StockQuantity);
        // descriptor.Field(v => v.Options).Type<ListType<VariantOptionType>>();
        descriptor.Field(v => v.Options);
        descriptor.Field(p => p.Media).Type<ListType<MediaType>>();
    }
}
