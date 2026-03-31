using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Api.Extensions;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.DeleteCategory;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.UpdateCategory;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.DeleteProduct;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.ToggleProductStatus;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateStock;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Products;

namespace VertexCommerce.Api.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog")
            .WithTags("Catalog");
            // .RequireAuthorization("Admin");
            
        // Products
        group.MapPost("/products", CreateProduct);
        group.MapPut("/products/{id:guid}", UpdateProduct);
        group.MapGet("/products/{id:guid}", GetProductById);
        group.MapDelete("/products/{id:guid}", DeleteProduct);
        group.MapPatch("/products/{id:guid}/stock", UpdateStock);
        group.MapPatch("/products/{id:guid}/stock/add", AddStock);
        group.MapPatch("/products/{id:guid}/stock/remove", RemoveStock);
        group.MapPost("/products/{id:guid}/activate", ActivateProduct);
        group.MapPost("/products/{id:guid}/deactivate", DeactivateProduct);

        // Categories
        group.MapPost("/categories", CreateCategory);
        group.MapPut("/categories/{id:guid}", UpdateCategory);
        group.MapDelete("/categories/{id:guid}", DeleteCategory);

        // Sync (MongoDB)
        group.MapPost("/sync", SyncAllProducts);
        group.MapPost("/sync/category/{categoryId:guid}", SyncCategoryProducts);

        return app;
    }

    #region Products

    private static async Task<IResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/catalog/products/{result.Value}", new { Id = result.Value })
            : result.Error.ToHttpResult();
    }
    
    private static async Task<IResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.CategoryId,
            request.Attributes,
            request.SeoMetadata,
            request.Variants
            );
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetProductById(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetProductByIdQuery(id);
        var result = await sender.Send(query, ct);
        
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }
    
    
    private static async Task<IResult> DeleteProduct(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeleteProductCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> UpdateStock(
        Guid id,
        [FromBody] UpdateStockRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new UpdateStockCommand(id, request.Quantity), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> AddStock(
        Guid id,
        [FromBody] StockQuantityRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new AddStockCommand(id, request.Quantity), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> RemoveStock(
        Guid id,
        [FromBody] StockQuantityRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new RemoveStockCommand(id, request.Quantity), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> ActivateProduct(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ActivateProductCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> DeactivateProduct(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeactivateProductCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    #endregion

    #region Categories

    private static async Task<IResult> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description,
            request.ParentId,
            request.SortOrder
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/catalog/categories/{result.Value}", new { Id = result.Value })
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(
            id,
            request.Name,
            request.Description,
            request.ParentId,
            request.SortOrder
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> DeleteCategory(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    #endregion

    #region Sync

    private static async Task<IResult> SyncAllProducts(
        [FromServices] IProductSyncService syncService,
        CancellationToken ct)
    {
        await syncService.SyncAllProductsAsync(ct);
        return Results.Ok(new { Message = "All products synced to MongoDB" });
    }

    private static async Task<IResult> SyncCategoryProducts(
        Guid categoryId,
        [FromServices] IProductSyncService syncService,
        CancellationToken ct)
    {
        await syncService.SyncCategoryProductsAsync(categoryId, ct);
        return Results.Ok(new { Message = $"Category {categoryId} products synced" });
    }

    #endregion
}