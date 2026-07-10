using VertexCommerce.Modules.Catalog.Features.Categories.Queries.GetCategoryById.DTOs;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IQuery<CategoryDto>;
