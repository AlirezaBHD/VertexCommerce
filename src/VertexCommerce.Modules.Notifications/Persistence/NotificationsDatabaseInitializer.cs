using Microsoft.EntityFrameworkCore;

namespace VertexCommerce.Modules.Notifications.Persistence;

internal static class NotificationsDatabaseInitializer
{
    private const string ReconcileInitialMigrationSql =
        "DO $$\n" +
        "BEGIN\n" +
        "    IF EXISTS (\n" +
        "        SELECT 1 FROM information_schema.tables\n" +
        "        WHERE table_schema = 'notifications' AND table_name = '__EFMigrationsHistory'\n" +
        "    ) THEN\n" +
        "        UPDATE notifications.\"__EFMigrationsHistory\"\n" +
        "        SET \"MigrationId\" = '20260920090000_InitialNotifications'\n" +
        "        WHERE \"MigrationId\" = '20260920120000_InitialNotifications';\n" +
        "    END IF;\n" +
        "END $$;";

    public static async Task ApplyMigrationsAsync(NotificationsDbContext dbContext, CancellationToken ct)
    {
        // Existing databases may have recorded the initial migration under its original ID.
        await dbContext.Database.ExecuteSqlRawAsync(ReconcileInitialMigrationSql, ct);
        await dbContext.Database.MigrateAsync(ct);
    }
}
