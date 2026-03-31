using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await productRepository.GetByIdWithVariantsAsync(command.Id, ct);
        if (product is null)
        {
            return Result.Failure(Error.NotFound("Product", command.Id));
        }

        if (!await categoryRepository.ExistsAsync(command.CategoryId, ct))
        {
            return Result.Failure(Error.NotFound("Category", command.CategoryId));
        }

        var slugValidation = await ValidateSlugAsync(product, command.SeoMetadata.Slug, ct);
        if (slugValidation.IsFailure)
        {
            return slugValidation;
        }

        if (command.Variants is { Count: > 0 })
        {
            var variantSync = new VariantSynchronizer(product, productRepository);
            await variantSync.SyncAsync(command.Variants, ct);
        }

        var seoMetadata = SeoMetadata.Create(
            command.SeoMetadata.Slug,
            command.SeoMetadata.MetaTitle,
            command.SeoMetadata.MetaDescription,
            command.SeoMetadata.Keywords);

        if (command.Attributes is not null)
        {
            product.UpdateAttributes(command.Attributes);
        }

        product.Update(command.Name, command.Description, command.CategoryId, seoMetadata);
        
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<Result> ValidateSlugAsync(Product product, string newSlug, CancellationToken ct)
    {
        if (product.Seo.Slug == newSlug)
        {
            return Result.Success();
        }

        if (await productRepository.SlugExistsAsync(newSlug, ct))
        {
            return Result.Failure(Error.Conflict($"Product Slug '{newSlug}' already exists."));
        }

        return Result.Success();
    }
}
