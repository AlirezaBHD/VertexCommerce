using MediatR;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.IntegrationEvents;

namespace VertexCommerce.Modules.Customers.Features.Customers.EventHandlers;

public class UserCreatedEventHandler(ICustomerRepository customerRepository, ICustomerUnitOfWork unitOfWork) : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(
            userId: notification.UserId,
            phoneNumber: PhoneNumber.Create(notification.PhoneNumber),
            firstName: FirstName.Create(notification.FirstName),
            lastName: LastName.Create(notification.LastName));

        await customerRepository.AddAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
