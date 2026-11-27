using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Modules.Notifications.Services.Dispatching;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Notifications.Tests;

internal sealed class NotificationTestDependencies
{
    public INotificationRepository Notifications { get; } = Substitute.For<INotificationRepository>();
    public IPushSubscriptionRepository Subscriptions { get; } = Substitute.For<IPushSubscriptionRepository>();
    public INotificationsUnitOfWork UnitOfWork { get; } = Substitute.For<INotificationsUnitOfWork>();
    public RecordingOrderNotificationInbox OrderInbox { get; } = new();
    public ICustomerService Customers { get; } = Substitute.For<ICustomerService>();
    public IIdentityService Identity { get; } = Substitute.For<IIdentityService>();

    public NotificationTestDependencies()
    {
        Identity.GetAdminUserIdsAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<Guid>());
    }

    public void Register(IServiceCollection services, bool usePostgres)
    {
        if (!usePostgres)
        {
            services.AddSingleton(Notifications);
            services.AddSingleton(Subscriptions);
            services.AddSingleton(UnitOfWork);
            services.AddSingleton<IOrderNotificationInbox>(OrderInbox);
        }

        services.AddSingleton(Customers);
        services.AddSingleton(Identity);
    }
}
