using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Products.Types;

public sealed class ProductType : ObjectType<ProductReadModel>
{
    protected override void Configure(
        IObjectTypeDescriptor<ProductReadModel> descriptor)
    {
        descriptor.Name("Product");

        descriptor.Field(p => p.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(p => p.Name).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Description).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.MinPrice).Type<NonNullType<DecimalType>>();
        descriptor.Field(p => p.MaxPrice).Type<NonNullType<DecimalType>>();
        descriptor.Field(p => p.TotalStock).Type<NonNullType<IntType>>();

        descriptor
            .Field("inStock")
            .Type<NonNullType<BooleanType>>()
            .Resolve(ctx =>
                ctx.Parent<ProductReadModel>().TotalStock > 0);

        descriptor.Field(p => p.IsActive).Type<NonNullType<BooleanType>>();

        descriptor.Field(p => p.CategoryId).Type<NonNullType<UuidType>>();
        descriptor.Field(p => p.CategoryName).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.CategoryPath).Type<NonNullType<StringType>>();

        descriptor.Field(p => p.Variants).Type<ListType<ProductVariantType>>();
        descriptor.Field(p => p.AvailableOptions).Ignore();

        descriptor.Field("availableOption")
            .Type<ListType<AvailableOptionType>>()
            .Resolve(ctx =>
            {
                var dict = ctx.Parent<ProductReadModel>().AvailableOptions;
                if (dict == null) return new List<AvailableOption>();

                return dict.Select(x => new AvailableOption
                {
                    Key = x.Key,
                    Values = x.Value
                }).ToList();
            });

        descriptor.Field(p => p.Media).Type<NonNullType<ListType<NonNullType<ProductMediaType>>>>();
        descriptor.Field(p => p.SearchText).Ignore();

        descriptor.Field(p => p.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(p => p.SyncedAt).Ignore();

        descriptor.Field(p => p.Slug).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.MetaTitle).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.MetaDescription).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Keywords).Type<NonNullType<StringType>>();
    }
}

public sealed class AvailableOptionType : ObjectType<AvailableOption>
{
    protected override void Configure(IObjectTypeDescriptor<AvailableOption> descriptor)
    {
        descriptor.Field(x => x.Key).Type<NonNullType<StringType>>();
        descriptor.Field(x => x.Values).Type<ListType<NonNullType<StringType>>>();
    }
}

public sealed class AvailableOption
{
    public string Key { get; set; } = default!;
    public List<string> Values { get; set; } = [];
}
