using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork)
    : ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand command, CancellationToken ct)
    {
        var categoryExists = await categoryRepository.ExistsAsync(command.CategoryId, ct);
        if (!categoryExists)
            return Result.Failure<CreateProductResponse>(Error.NotFound("Category", command.CategoryId));

        if (await productRepository.SlugExistsAsync(command.SeoMetadata.Slug, ct))
            return Result.Failure<CreateProductResponse>(
                Error.Conflict($"Product Slug '{command.SeoMetadata.Slug}' already exists."));

        
        var seoMetadata = SeoMetadata.Create(
            command.SeoMetadata.Slug,
            command.SeoMetadata.MetaTitle,
            command.SeoMetadata.MetaDescription,
            command.SeoMetadata.Keywords);
        
        
        var product = Product.Create(
            command.Name,
            command.Description,
            command.CategoryId,
            seoMetadata
        );
        
        if (command.Attributes is not null)
        {
            foreach (var attr in command.Attributes)
                product.AddAttribute(attr.Key, attr.Value);
        }
        
        
        var variantInfos = new List<VariantInfo>();

        if (command.Variants is not null)
        {
            foreach (var v in command.Variants)
            {
                // if (await _productRepository.SkuExistsAsync(v.Sku, ct))
                //     return Result.Failure<CreateProductResponse>(
                //         Error.Conflict($"Variant SKU '{v.Sku}' already exists."));

                var options = v.Options
                    .Select(o => VariantOption.Create(o.Name, o.Value))
                    .ToList();

                var price = Money.Create(v.Price, v.Currency ?? "USD");

                var sku = Sku.Generate();
                var variant = ProductVariant.Create(
                    product.Id,
                    sku,
                    options,
                    v.StockQuantity,
                    v.Order,
                    price
                );
                var variantMediaList = v.Medias.Select(media => ProductMedia.Create(media.Path, MediaType.Image, media.Order)).ToList();//TODO hardcoded Image type.
                variant.SetMedia(variantMediaList);

                product.AddVariant(variant);

                variantInfos.Add(new VariantInfo(variant.Id, sku.Value));
            }
        }
        
        
        await productRepository.AddAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success(
            new CreateProductResponse(product.Id, variantInfos));
        
    }
}