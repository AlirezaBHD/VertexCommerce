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
            return Result.Failure(Error.Conflict("Slug already exists."));
        }

        // Update variants
        if (command.Variants is { Count: > 0 })
        {
            var existingVariantIds = command.Variants
                .Where(v => v.Id.HasValue)
                .Select(v => v.Id!.Value)
                .ToHashSet();

            // Remove deleted variants
            var variantsToRemove = product.Variants
                .Where(v => !existingVariantIds.Contains(v.Id))
                .Select(v => v.Id)
                .ToList();

            foreach (var variantId in variantsToRemove)
            {
                product.RemoveVariant(variantId);
            }

            // Add or update variants
            foreach (var variantDto in command.Variants)
            {
                var attributes = variantDto.Attributes
                    .Select(a => ProductAttribute.Create(a.AttributeCode, a.OptionCode))
                    .ToList();
            
                if (variantDto.Id.HasValue)
                {
                    // // Update existing variant
                    var existingVariant = product.Variants.FirstOrDefault(v => v.Id == variantDto.Id.Value);
                    if (existingVariant is not null)
                    {
                        var sku = !string.IsNullOrEmpty(variantDto.Sku) 
                            ? Sku.Create(variantDto.Sku) 
                            : existingVariant.Sku;
                        
                        var price = Money.Create(variantDto.Price, variantDto.Currency ?? "USD");
                        
                        existingVariant.Update(
                            sku: sku,
                            stockQuantity: variantDto.StockQuantity,
                            order: variantDto.SortOrder,
                            price: price,
                            attributes: attributes);
                         productRepository.UpdateVariantAsync(existingVariant);

                    }
                }
                else
                {
                    // Create new variant
                    var sku = !string.IsNullOrEmpty(variantDto.Sku)
                                                ? Sku.Create(variantDto.Sku)
                        : Sku.Generate();
            
                    var price = Money.Create(variantDto.Price, variantDto.Currency ?? "USD");
            
                    var newVariant = ProductVariant.Create(
                        productId: product.Id,
                        sku: sku,
                        stockQuantity: variantDto.StockQuantity,
                        order: variantDto.SortOrder,
                        price: price,
                        attributes: attributes);
            
                    product.AddVariant(newVariant);
                    await productRepository.AddVariantAsync(newVariant, ct);
                }
            }
        }

        // Update media
        if (command.Media is { Count: > 0 })
        {
            var medias = command.Media.Select(m =>
                ProductMedia.Create(
                    path: m.Path,
                    type: MediaType.Image,
                    order: m.SortOrder,
                    altText: m.AltText,
                    associatedAttributeCode: m.AssociatedAttributeCode,
                    associatedOptionCode: m.AssociatedOptionCode
                )).ToList();

            product.SetMedia(medias);
        }

        var seoMetadata = SeoMetadata.Create(
            command.SeoMetadata.Slug,
            command.SeoMetadata.MetaTitle,
            command.SeoMetadata.MetaDescription,
            command.SeoMetadata.Keywords);

        product.Update(
            name: command.Name,
            description: command.Description,
            categoryId: command.CategoryId,
            seoMetadata: seoMetadata);

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
