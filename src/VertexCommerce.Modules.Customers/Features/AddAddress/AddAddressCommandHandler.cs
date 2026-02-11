using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.GetCustomer;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.AddAddress;

internal sealed class AddAddressCommandHandler : ICommandHandler<AddAddressCommand, AddressResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerUnitOfWork _unitOfWork;

    public AddAddressCommandHandler(
        ICustomerRepository customerRepository,
        ICustomerUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddressResponse>> Handle(AddAddressCommand command, CancellationToken ct)
    {
        var customer = await _customerRepository.GetByUserIdAsync(command.UserId, ct);

        if (customer is null)
            return Result.Failure<AddressResponse>(Error.NotFound("Customer", command.UserId));

        var address = customer.AddAddress(
            command.Street,
            command.City,
            command.State,
            command.Country,
            command.ZipCode,
            command.Label
        );

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AddressResponse(
            address.Id,
            address.Street,
            address.City,
            address.State,
            address.Country,
            address.ZipCode,
            address.Label,
            address.FullAddress
        ));
    }
}
