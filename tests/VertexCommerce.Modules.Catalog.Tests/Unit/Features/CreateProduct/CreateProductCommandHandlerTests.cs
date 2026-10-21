using NSubstitute;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Catalog.Tests.Fixtures;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Features.CreateProduct;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _unitOfWork = Substitute.For<ICatalogUnitOfWork>();

        _handler = new CreateProductCommandHandler(
            _productRepository,
            _categoryRepository,
            _unitOfWork
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProductAndReturnResponse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = CatalogFixtures.Products.CreateCommand(categoryId: categoryId);

        _categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(true);
        _productRepository.SlugExistsAsync(command.SeoMetadata.Slug, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ProductId.Should().NotBeEmpty();
        result.Value.Variants.Should().HaveCount(1);
        result.Value.Variants[0].Sku.Should().StartWith("PRD-");

        await _productRepository.Received(1).AddAsync(
            Arg.Is<Product>(p => p.Name.Value == command.Name && p.CategoryId == categoryId),
            Arg.Any<CancellationToken>()
        );
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldReturnFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = CatalogFixtures.Products.CreateCommand(categoryId: categoryId);

        _categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");

        await _productRepository.DidNotReceive().AddAsync(
            Arg.Any<Product>(),
            Arg.Any<CancellationToken>()
        );
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSlugAlreadyExists_ShouldReturnConflictFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = CatalogFixtures.Products.CreateCommand(categoryId: categoryId);

        _categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(true);
        _productRepository.SlugExistsAsync(command.SeoMetadata.Slug, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Conflict");

        await _productRepository.DidNotReceive().AddAsync(
            Arg.Any<Product>(),
            Arg.Any<CancellationToken>()
        );
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithVariantsAndMedia_ShouldAttachVariantsAndMediaToProduct()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand(
            Name: "Smartphone Pro",
            Description: "Flagship phone",
            CategoryId: categoryId,
            SeoMetadata: new SeoMetadataRequest("smartphone-pro", "Meta Title", "Meta Desc", "phone,tech"),
            Variants:
            [
                new CreateVariantRequest(
                    Price: 999m,
                    Currency: "USD",
                    StockQuantity: 50,
                    SortOrder: 0,
                    Attributes: [new ProductAttributeRequest("COLOR", "BLACK")]
                )
            ],
            Media:
            [
                new CreateMediaRequest(
                    Path: "/images/phone.png",
                    SortOrder: 0,
                    AltText: "Front view",
                    AssociatedAttributeCode: null,
                    AssociatedOptionCode: null
                )
            ]
        );

        _categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(true);
        _productRepository.SlugExistsAsync(command.SeoMetadata.Slug, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Variants.Should().HaveCount(1);

        await _productRepository.Received(1).AddAsync(
            Arg.Is<Product>(p =>
                p.Name.Value == "Smartphone Pro" &&
                p.Variants.Count == 1 &&
                p.Media.Count == 1 &&
                p.Variants.First().StockQuantity == 50),
            Arg.Any<CancellationToken>()
        );
    }
}