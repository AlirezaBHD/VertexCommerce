using NSubstitute;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Modules.Catalog.Tests.Fixtures;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Features.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _unitOfWork = Substitute.For<ICatalogUnitOfWork>();

        _handler = new CreateCategoryCommandHandler(
            _categoryRepository,
            _unitOfWork
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateCategoryAndReturnId()
    {
        // Arrange
        var command = CatalogFixtures.Categories.CreateCommand();

        _categoryRepository.NameExistsAsync(command.Name, null, Arg.Any<CancellationToken>())
            .Returns(false);
        _categoryRepository.SlugExistsAsync(command.Seo.Slug, null, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        await _categoryRepository.Received(1).AddAsync(
            Arg.Is<Category>(c => c.Name.Value == command.Name && c.SortOrder == command.SortOrder),
            Arg.Any<CancellationToken>()
        );
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenParentIdSpecifiedAndDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var command = CatalogFixtures.Categories.CreateCommand(parentId: parentId);

        _categoryRepository.ExistsAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");

        await _categoryRepository.DidNotReceive().AddAsync(
            Arg.Any<Category>(),
            Arg.Any<CancellationToken>()
        );
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryNameAlreadyExists_ShouldReturnConflictError()
    {
        // Arrange
        var command = CatalogFixtures.Categories.CreateCommand(name: "Existing Category");

        _categoryRepository.NameExistsAsync("Existing Category", null, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Conflict");

        await _categoryRepository.DidNotReceive().AddAsync(
            Arg.Any<Category>(),
            Arg.Any<CancellationToken>()
        );
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSlugAlreadyExists_ShouldReturnConflictError()
    {
        // Arrange
        var command = CatalogFixtures.Categories.CreateCommand(slug: "existing-slug");

        _categoryRepository.NameExistsAsync(command.Name, null, Arg.Any<CancellationToken>())
            .Returns(false);
        _categoryRepository.SlugExistsAsync("existing-slug", null, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Conflict");

        await _categoryRepository.DidNotReceive().AddAsync(
            Arg.Any<Category>(),
            Arg.Any<CancellationToken>()
        );
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
