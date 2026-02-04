using FluentAssertions;
using NSubstitute;
using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Modules.Catalog.Domain.Repositories;
using VertexCommerce.Modules.Catalog.Features.CreateProduct;
using VertexCommerce.Modules.Catalog.Tests.Fixtures;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Features.CreateProduct;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new CreateProductCommandHandler(
            _productRepository,
            _categoryRepository,
            _unitOfWork
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateProduct()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = CatalogFixtures.Products.CreateCommand(categoryId: categoryId);

        _categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(true);
        _productRepository.SkuExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        await _productRepository.Received(1).AddAsync(
            Arg.Is<Product>(p => p.Name == command.Name),
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
    }

    [Fact]
    public async Task Handle_WhenSkuExists_ShouldReturnFailure()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "Test Product",
            null,
            "EXISTING-SKU",
            99.99m,
            "USD",
            10,
            categoryId
        );

        _categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(true);
        _productRepository.SkuExistsAsync("EXISTING-SKU", Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Conflict");
    }

    [Fact]
    public async Task Handle_WithoutSku_ShouldGenerateSku()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "Test Product",
            null,
            null, // No SKU provided
            99.99m,
            "USD",
            10,
            categoryId
        );

        _categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(true);
        _productRepository.SkuExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _productRepository.Received(1).AddAsync(
            Arg.Is<Product>(p => p.Sku.Value.StartsWith("PRD-")),
            Arg.Any<CancellationToken>()
        );
    }
}