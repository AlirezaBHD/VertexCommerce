using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.EditAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.RemoveAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;
using VertexCommerce.Modules.Customers.Features.Customers.Commands.CreateCustomer;
using VertexCommerce.Modules.Customers.Features.Customers.Commands.SetDefaultBillingAddress;
using VertexCommerce.Modules.Customers.Features.Customers.Commands.SetDefaultShippingAddress;
using VertexCommerce.Modules.Customers.Features.Customers.Commands.UpdateCustomer;
using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;
using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerAdmin;
using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomers;
using VertexCommerce.Shared.Extensions;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminAddAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminEditAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminRemoveAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminSetDefaultAddress;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Customers.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers")
            .WithTags("Customers")
            .RequireAuthorization();

        group.MapGet("/me", GetMyProfile);
        group.MapPost("/me/addresses", AddAddress);
        group.MapDelete("/me/addresses/{id:guid}", RemoveAddress);
        group.MapGet("/me/addresses/{id:guid}", GetAddressById);
        group.MapPut("/me/addresses/{id:guid}", EditAddress);
        group.MapPatch("/me/addresses/{id:guid}/set-default-shipping", SetDefaultShippingAddress);
        group.MapPatch("/me/addresses/{id:guid}/set-default-billing", SetDefaultBillingAddress);

        var adminGroup = app.MapGroup("/api/admin/customers")
            .WithTags("Customers")
            .RequireAuthorization(AppRoles.Admin);

        adminGroup.MapGet("/", GetCustomers);
        adminGroup.MapGet("/{id:guid}", GetCustomerAdmin);
        adminGroup.MapPost("/", CreateCustomer);
        adminGroup.MapPut("/{id:guid}", UpdateCustomer);
        adminGroup.MapPost("/{id:guid}/addresses", AdminAddAddress);
        adminGroup.MapPut("/{id:guid}/addresses/{addressId:guid}", AdminEditAddress);
        adminGroup.MapDelete("/{id:guid}/addresses/{addressId:guid}", AdminRemoveAddress);
        adminGroup.MapPatch("/{id:guid}/addresses/{addressId:guid}/set-default", AdminSetDefaultAddress);

        return app;
    }

    private static async Task<IResult> GetCustomers(
        [FromQuery] string? searchTerm,
        [FromQuery] string? sortBy,
        [FromQuery] bool? sortDescending,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetCustomersQuery(
            SearchTerm: searchTerm,
            SortBy: sortBy,
            SortDescending: sortDescending ?? true)
        {
            Page = page,
            PageSize = pageSize
        };
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetCustomerAdmin(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetCustomerAdminQuery(CustomerId: id);
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> CreateCustomer(
        [FromBody] CreateCustomerCommand command,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/admin/customers/{result.Value.Id}", result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> UpdateCustomer(
        Guid id,
        [FromBody] UpdateCustomerRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateCustomerCommand(
            CustomerId: id,
            PhoneNumber: request.PhoneNumber,
            FirstName: request.FirstName,
            LastName: request.LastName);

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> SetDefaultShippingAddress(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new SetDefaultShippingAddressCommand(AddressId: id);
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }
    private static async Task<IResult> SetDefaultBillingAddress(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new SetDefaultBillingAddressCommand(AddressId: id);
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetAddressById(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetAddressByIdQuery(AddressId: id);
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> RemoveAddress(
        Guid id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new RemoveAddressCommand(AddressId: id);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> GetMyProfile(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var query = new GetCustomerQuery();
        var result = await sender.Send(query, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> AddAddress(
        [FromBody] AddAddressRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new AddAddressCommand(
            Province: request.Province,
            City: request.City,
            PostalAddress: request.PostalAddress,
            PostalCode: request.PostalCode,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            Label: request.Label
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/customers/me/addresses/{result.Value.Id}", result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> EditAddress(
        Guid id,
        [FromBody] AddAddressRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new EditAddressCommand(
            AddressId: id,
            Province: request.Province,
            City: request.City,
            PostalAddress: request.PostalAddress,
            PostalCode: request.PostalCode,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            Label: request.Label
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> AdminAddAddress(
        Guid id,
        [FromBody] AddAddressRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new AdminAddAddressCommand(
            CustomerId: id,
            Province: request.Province,
            City: request.City,
            PostalAddress: request.PostalAddress,
            PostalCode: request.PostalCode,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            Label: request.Label
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/admin/customers/{id}/addresses/{result.Value.Id}", result.Value)
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> AdminEditAddress(
        Guid id,
        Guid addressId,
        [FromBody] AddAddressRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new AdminEditAddressCommand(
            CustomerId: id,
            AddressId: addressId,
            Province: request.Province,
            City: request.City,
            PostalAddress: request.PostalAddress,
            PostalCode: request.PostalCode,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            Label: request.Label
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> AdminRemoveAddress(
        Guid id,
        Guid addressId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new AdminRemoveAddressCommand(
            CustomerId: id,
            AddressId: addressId
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }

    private static async Task<IResult> AdminSetDefaultAddress(
        Guid id,
        Guid addressId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new AdminSetDefaultAddressCommand(
            CustomerId: id,
            AddressId: addressId
        );

        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();
    }
}
