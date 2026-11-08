using HotChocolate.Authorization;
using VertexCommerce.Modules.Catalog.GraphQL.Categories.Types;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Catalog.GraphQL.Products.Types;

public sealed class AvailableOptionType : ObjectType<AvailableOption>
{
    protected override void Configure(IObjectTypeDescriptor<AvailableOption> descriptor)
    {
        descriptor.Field(x => x.Key).Type<NonNullType<StringType>>();
        descriptor.Field(x => x.Values).Type<ListType<NonNullType<StringType>>>();
    }
}
