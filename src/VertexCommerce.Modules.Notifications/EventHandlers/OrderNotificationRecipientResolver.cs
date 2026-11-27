using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using VertexCommerce.Shared.Contracts.Customers;

namespace VertexCommerce.Modules.Notifications.EventHandlers;

internal sealed class OrderNotificationRecipientResolver(
    ICustomerService customerService,
    ILogger<OrderNotificationRecipientResolver> logger)
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Order notification delivery uses the event's customer ID if customer lookup fails")]
    public async Task<(Guid UserId, string DisplayName)> ResolveAsync(
        Guid customerId, CancellationToken cancellationToken)
    {
        CustomerInfoDto? customer = null;
        try
        {
            customer = await customerService.GetCustomerInfo(customerId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not fetch customer info for CustomerId {CustomerId}", customerId);
        }

        var displayName = customer is not null &&
                          (!string.IsNullOrWhiteSpace(customer.FirstName) || !string.IsNullOrWhiteSpace(customer.LastName))
            ? $"{customer.FirstName} {customer.LastName}".Trim()
            : customer?.PhoneNumber ?? "مشتری";

        return (customer?.UserId ?? customerId, displayName);
    }
}
