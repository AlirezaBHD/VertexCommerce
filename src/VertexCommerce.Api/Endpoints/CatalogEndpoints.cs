using MediatR;
using Microsoft.AspNetCore.Mvc;
using VertexCommerce.Modules.Catalog.Features.CreateCategory;
using VertexCommerce.Modules.Catalog.Features.CreateProduct;
using VertexCommerce.Modules.Catalog.Features.GetProductById;
using VertexCommerce.Modules.Catalog.Features.GetProducts;

namespace VertexCommerce.Api.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalog")
            .WithTags("Catalog");

        // Products
        group.MapPost("/products", CreateProduct)
            .WithName("CreateProduct")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/products", GetProducts)
            .WithName("GetProducts")
            .Produces<PagedResult<ProductListItem>>();

        group.MapGet("/products/{id:guid}", GetProductById)
            .WithName("GetProductById")
            .Produces<ProductResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        // Categories
        group.MapPost("/categories", CreateCategory)
            .WithName("CreateCategory")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/catalog/products/{result.Value}", result.Value)
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: GetStatusCode(result.Error.Code));
    }

    private static async Task<IResult> GetProducts(
        [AsParameters] GetProductsRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var query = new GetProductsQuery(
            request.SearchTerm,
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            request.IsActive,
            request.Page ?? 1,
            request.PageSize ?? 20
        );

        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: GetStatusCode(result.Error.Code));
    }

    private static async Task<IResult> GetProductById(
        Guid id,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/catalog/categories/{result.Value}", result.Value)
            : Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: GetStatusCode(result.Error.Code));
    }

    private static int GetStatusCode(string errorCode) => errorCode switch
    {
        _ when errorCode.Contains("NotFound") => StatusCodes.Status404NotFound,
        _ when errorCode.Contains("Conflict") => StatusCodes.Status409Conflict,
        _ when errorCode.Contains("Validation") => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };
}

public record GetProductsRequest(
    string? SearchTerm = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsActive = null,
    int? Page = null,
    int? PageSize = null
);