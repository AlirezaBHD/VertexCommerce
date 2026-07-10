using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Features.Categories.Queries.GetCategoryById.DTOs;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    : IQueryHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery query, CancellationToken ct)
    {
        var spec = new GetCategoryByIdSpec(query.Id);
        var category = await categoryRepository.GetByIdAsync(query.Id, spec, ct);

        if (category is null)
        {
            return Result.Failure<CategoryDto>(Error.NotFound("Category", query.Id));
        }

        return Result.Success(category);
    }
}
