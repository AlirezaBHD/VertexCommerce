using VertexCommerce.Modules.Basket.Persistence.Documents;

namespace VertexCommerce.Modules.Basket.GraphQL.Types;

public sealed class BasketItemType : ObjectType<BasketItemDocument>
{
    protected override void Configure(
        IObjectTypeDescriptor<BasketItemDocument> descriptor)
    {
        descriptor.Name("BasketItem");
        descriptor.Field(b => b.ProductId).Type<NonNullType<UuidType>>();
        descriptor.Field(b => b.VariantId).Type<NonNullType<UuidType>>();
        descriptor.Field(b => b.ProductName).Type<NonNullType<StringType>>();
        descriptor.Field(b => b.Sku).Type<StringType>();
        descriptor.Field(b => b.ImagePath).Type<StringType>();
        descriptor.Field(b => b.Price).Type<NonNullType<DecimalType>>();
        descriptor.Field(b => b.Quantity).Type<NonNullType<IntType>>();
        descriptor.Field(b => b.StockQuantity).Type<NonNullType<IntType>>();
        descriptor.Field(b => b.TotalPrice).Type<NonNullType<DecimalType>>();
        descriptor.Field(b => b.AddedAt).Type<NonNullType<DateTimeType>>();
    }
}