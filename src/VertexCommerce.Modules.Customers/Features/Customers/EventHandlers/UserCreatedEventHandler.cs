using MediatR;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.IntegrationEvents;

namespace VertexCommerce.Modules.Customers.Features.Customers.EventHandlers;

public class UserCreatedEventHandler(CustomersDbContext customerDbContext) : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(
            userId: notification.UserId,
            phoneNumber: notification.PhoneNumber,
            firstName: notification.FirstName,
            lastName: notification.LastName);

        customerDbContext.Customers.Add(customer);
        await customerDbContext.SaveChangesAsync(cancellationToken);
    }
}
