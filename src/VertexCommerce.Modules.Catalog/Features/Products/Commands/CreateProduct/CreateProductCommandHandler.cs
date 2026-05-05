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

        if (command.Media is not null)
        {
            var mediaList = command.Media
                .Select(m => 
                    ProductMedia.Create(
                        m.Path, MediaType.Image,
                        m.SortOrder, m.AltText, 
                        m.AssociatedAttributeCode, m.AssociatedOptionCode))
                .ToList();
            product.SetMedia(mediaList);
        }

        var variantInfos = new List<VariantInfo>();

        if (command.Variants is not null)
        {
            foreach (var v in command.Variants)
            {
                var attributes = v.Attributes
                    .Select(a => ProductAttribute.Create(a.AttributeCode, a.OptionCode))
                    .ToList();

                var price = Money.Create(v.Price, v.Currency ?? "USD");
                var sku = Sku.Generate();

                var variant = ProductVariant.Create(
                    product.Id,
                    sku,
                    v.StockQuantity,
                    v.SortOrder,
                    price,
                    attributes
                );

                product.AddVariant(variant);
                variantInfos.Add(new VariantInfo(variant.Id, sku.Value));
            }
        }

        await productRepository.AddAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CreateProductResponse(product.Id, variantInfos));
    }
}
