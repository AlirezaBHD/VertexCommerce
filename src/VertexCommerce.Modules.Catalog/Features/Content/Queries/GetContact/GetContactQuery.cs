using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Queries.GetContact;

public sealed record GetContactQuery : IQuery<ContactDocument>;
