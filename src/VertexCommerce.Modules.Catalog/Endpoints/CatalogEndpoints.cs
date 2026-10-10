using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.DeleteCategory;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.ReorderCategories;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.UpdateCategory;
using VertexCommerce.Modules.Catalog.Features.Categories.Queries.GetCategoryById;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.DeleteProduct;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.ToggleProductStatus;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateProduct;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateStock;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetCatalogAttributes;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.Lookups;
using VertexCommerce.Modules.Catalog.Features.Categories.Queries.Lookups;
using VertexCommerce.Modules.Catalog.Features.Dashboard.GetProductStats;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Extensions;

namespace VertexCommerce.Modules.Catalog.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        var adminGroup = app.MapGroup("/api/catalog")
            .WithTags("Catalog")
            .RequireAuthorization(AppRoles.Admin);
        
        var publicGroup = app.MapGroup("/api/catalog")
            .WithTags("Catalog");
            
        // Products
        adminGroup.MapPost("/products", CreateProduct);
        adminGroup.MapPut("/products/{id:guid}", UpdateProduct);
        adminGroup.MapGet("/products/{id:guid}", GetProductById);
        adminGroup.MapDelete("/products/{id:guid}", DeleteProduct);
        adminGroup.MapPatch("/products/{id:guid}/stock", UpdateStock);
        adminGroup.MapPatch("/products/{id:guid}/stock/add", AddStock);
        adminGroup.MapPatch("/products/{id:guid}/stock/remove", RemoveStock);
        adminGroup.MapPost("/products/{id:guid}/activate", ActivateProduct);
        adminGroup.MapPost("/products/{id:guid}/deactivate", DeactivateProduct);
        adminGroup.MapGet("/products/lookup", ProductLookup);
        adminGroup.MapGet("/products/stats", GetProductStats);

        // Categories
        adminGroup.MapPost("/categories", CreateCategory);
        adminGroup.MapGet("/categories/{id:guid}", GetCategoryById);
        adminGroup.MapPut("/categories/{id:guid}", UpdateCategory);
        adminGroup.MapDelete("/categories/{id:guid}", DeleteCategory);
        adminGroup.MapPatch("/categories/reorder", ReorderCategories);
        adminGroup.MapGet("/categories/lookup", CategoryLookup);

        publicGroup.MapGet("/attributes", GetAttributes);


        return app;
    }
    
    private static async Task<IResult> GetAttributes(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetCatalogAttributesQuery();
        var result = await sender.Send(query, ct);
        
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
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
            request.SeoMetadata,
            request.Variants,
            request.Media
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

    private static async Task<IResult> ProductLookup(
        [FromQuery] string? q,
        [FromQuery] int limit,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetProductLookupQuery(q, limit);
        var result = await sender.Send(query, ct);
        
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetProductStats(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetProductStatsQuery();
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
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
            Name: request.Name,
            Description: request.Description,
            Seo: request.Seo,
            IconPath: request.IconPath,
            CoverImagePath: request.CoverImagePath,
            ImageAltText: request.ImageAltText,
            ParentId: request.ParentId,
            IsActive: request.IsActive,
            ShowOnHome: request.ShowOnHome,
            IncludeInMenu: request.IncludeInMenu,
            SortOrder: request.SortOrder
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/catalog/categories/{result.Value}", new { Id = result.Value })
            : result.Error.ToHttpResult();
    }
    
    private static async Task<IResult> GetCategoryById(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await sender.Send(query, ct);
        
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(
            Id: id,
            Name: request.Name,
            Description: request.Description,
            Seo: request.Seo,
            IconPath: request.IconPath,
            CoverImagePath: request.CoverImagePath,
            ImageAltText: request.ImageAltText,
            ParentId: request.ParentId,
            IsActive: request.IsActive,
            ShowOnHome: request.ShowOnHome,
            IncludeInMenu: request.IncludeInMenu
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

    private static async Task<IResult> ReorderCategories(
        [FromBody] ReorderCategoriesRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new ReorderCategoriesCommand(request.Items);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> CategoryLookup(
        [FromQuery] string? q,
        [FromQuery] int limit,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetCategoryLookupQuery(q, limit);
        var result = await sender.Send(query, ct);
        
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    #endregion
}