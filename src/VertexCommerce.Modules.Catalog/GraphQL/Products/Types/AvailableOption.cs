using HotChocolate.Authorization;
using VertexCommerce.Modules.Catalog.GraphQL.Categories.Types;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Catalog.GraphQL.Products.Types;

public sealed class AvailableOption
{
    public string Key { get; set; } = default!;
    public List<string> Values { get; set; } = [];
}
