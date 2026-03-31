using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Api.GraphQL.Catalog.Types;

public sealed class ProductType : ObjectType<ProductReadModel>
{
    protected override void Configure(
        IObjectTypeDescriptor<ProductReadModel> descriptor)
    {
        descriptor.Name("Product");

        descriptor.Field(p => p.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(p => p.Name).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Description).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.TotalStock).Type<NonNullType<IntType>>();
        descriptor.Field(p => p.Slug).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.MetaTitle).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.MetaDescription).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Keywords).Type<NonNullType<StringType>>();

        
        descriptor.Field(p => p.IsActive);
        descriptor.Field(p => p.SearchText).Ignore();
        descriptor.Field(p => p.SyncedAt).Ignore();

        descriptor
            .Field("inStock")
            .Type<NonNullType<BooleanType>>()
            .Resolve(ctx =>
                ctx.Parent<ProductReadModel>().TotalStock > 0);

        descriptor.Field(p => p.CategoryName).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Variants).Type<ListType<VariantType>>();
        descriptor.Field(p => p.CreatedAt).Type<NonNullType<DateTimeType>>();;
    }
}


// public sealed class VariantOptionType : ObjectType<VariantOptionReadModel>
// {
//     protected override void Configure(
//         IObjectTypeDescriptor<VariantOptionReadModel> descriptor)
//     {
//         descriptor.Name("VariantOption");
//
//         descriptor.Field(o => o.Name);
//         descriptor.Field(o => o.Value);
//     }
// }