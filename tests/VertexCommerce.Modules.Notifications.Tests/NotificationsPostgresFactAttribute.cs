using Xunit;

namespace VertexCommerce.Modules.Notifications.Tests;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class NotificationsPostgresFactAttribute : FactAttribute
{
    public const string ConnectionVariable = "VERTEX_NOTIFICATIONS_TEST_DATABASE";

    public NotificationsPostgresFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable)))
        {
            Skip = $"Set {ConnectionVariable} to a PostgreSQL test server connection string.";
        }
    }
}
