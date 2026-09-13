using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
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

        var slugValidation = await ValidateSlugAsync(product, Slug.Create(command.SeoMetadata.Slug), ct);
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
                    .Select(a => ProductAttribute.Create(AttributeCode.Create(a.AttributeCode), OptionCode.Create(a.OptionCode)))
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
                        
                        var price = Money.Create(variantDto.Price, Currency.Create(variantDto.Currency ?? "USD"));
                        
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
            
                    var price = Money.Create(variantDto.Price, Currency.Create(variantDto.Currency ?? "USD"));
            
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
                    path: ImagePath.Create(m.Path),
                    type: MediaType.Image,
                    order: m.SortOrder,
                    altText: AltText.CreateOrNull(m.AltText),
                    associatedAttributeCode: m.AssociatedAttributeCode != null ? AttributeCode.Create(m.AssociatedAttributeCode) : null,
                    associatedOptionCode: m.AssociatedOptionCode != null ? OptionCode.Create(m.AssociatedOptionCode) : null
                )).ToList();

            product.SetMedia(medias);
        }

        var seoMetadata = SeoMetadata.Create(
            Slug.Create(command.SeoMetadata.Slug),
            MetaTitle.CreateOrNull(command.SeoMetadata.MetaTitle),
            MetaDescription.CreateOrNull(command.SeoMetadata.MetaDescription),
            SeoKeywords.CreateOrNull(command.SeoMetadata.Keywords));

        product.Update(
            name: ProductName.Create(command.Name),
            description: command.Description != null ? ProductDescription.Create(command.Description) : null,
            categoryId: command.CategoryId,
            seoMetadata: seoMetadata);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<Result> ValidateSlugAsync(Product product, Slug newSlug, CancellationToken ct)
    {
        if (product.Seo.Slug.Value == newSlug.Value)
        {
            return Result.Success();
        }

        if (await productRepository.SlugExistsAsync(newSlug.Value, ct))
        {
            return Result.Failure(Error.Conflict($"Product Slug '{newSlug}' already exists."));
        }

        return Result.Success();
    }
}
