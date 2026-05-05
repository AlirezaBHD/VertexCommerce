using VertexCommerce.Modules.Basket.Persistence.Documents;

namespace VertexCommerce.Modules.Basket.GraphQL.Types;

public sealed class BasketType : ObjectType<BasketDocument>
{
    protected override void Configure(
        IObjectTypeDescriptor<BasketDocument> descriptor)
    {
        descriptor.Name("Basket");
        descriptor.Field(b => b.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(b => b.CustomerId).Type<NonNullType<UuidType>>();
        descriptor.Field(b => b.TotalAmount).Type<NonNullType<DecimalType>>();
        descriptor.Field(b => b.TotalItems).Type<NonNullType<IntType>>();
        descriptor.Field(b => b.Items).Type<NonNullType<ListType<NonNullType<BasketItemType>>>>();
        descriptor.Field(b => b.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(b => b.ExpiresAt).Type<DateTimeType>();
    }
}
