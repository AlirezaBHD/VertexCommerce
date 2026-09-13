using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
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
                Error.Conflict($"Product Slug '{Slug.Create(command.SeoMetadata.Slug)}' already exists."));

        var seoMetadata = SeoMetadata.Create(
            Slug.Create(command.SeoMetadata.Slug),
            MetaTitle.CreateOrNull(command.SeoMetadata.MetaTitle),
            MetaDescription.CreateOrNull(command.SeoMetadata.MetaDescription),
            SeoKeywords.CreateOrNull(command.SeoMetadata.Keywords));

        var product = Product.Create(
            ProductName.Create(command.Name),
            ProductDescription.CreateOrNull(command.Description),
            command.CategoryId,
            seoMetadata
        );

        if (command.Media is not null)
        {
            var mediaList = command.Media
                .Select(m => 
                    ProductMedia.Create(ImagePath.Create(m.Path), MediaType.Image,
                        m.SortOrder, AltText.CreateOrNull(m.AltText), m.AssociatedAttributeCode != null ? AttributeCode.Create(m.AssociatedAttributeCode) : null, m.AssociatedOptionCode != null ? OptionCode.Create(m.AssociatedOptionCode) : null))
                .ToList();
            product.SetMedia(mediaList);
        }

        var variantInfos = new List<VariantInfo>();

        if (command.Variants is not null)
        {
            foreach (var v in command.Variants)
            {
                var attributes = v.Attributes
                    .Select(a => ProductAttribute.Create(AttributeCode.Create(a.AttributeCode), OptionCode.Create(a.OptionCode)))
                    .ToList();

                var price = Money.Create(v.Price, Currency.Create(v.Currency ?? "USD"));
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
