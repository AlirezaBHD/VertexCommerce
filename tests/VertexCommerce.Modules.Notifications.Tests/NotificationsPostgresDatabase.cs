using System.Diagnostics.CodeAnalysis;
using Npgsql;

namespace VertexCommerce.Modules.Notifications.Tests;

internal sealed class NotificationsPostgresDatabase(
    string adminConnection, string databaseName, string connectionString) : IAsyncDisposable
{
    public string ConnectionString { get; } = connectionString;

    public static async Task<NotificationsPostgresDatabase> CreateAsync(CancellationToken ct)
    {
        var configured = Environment.GetEnvironmentVariable(NotificationsPostgresFactAttribute.ConnectionVariable);
        var options = new NpgsqlConnectionStringBuilder(configured) { Pooling = false };
        var adminConnection = options.ConnectionString;
        var database = $"vertex_notifications_{Guid.NewGuid():N}";

        await ExecuteAsync(adminConnection, $"CREATE DATABASE {database}", ct);

        options.Database = database;
        return new NotificationsPostgresDatabase(adminConnection, database, options.ConnectionString);
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask(ExecuteAsync(adminConnection, $"DROP DATABASE {databaseName} WITH (FORCE)", CancellationToken.None));
    }

    [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "Commands use fixed SQL and a database identifier generated from a test GUID in hexadecimal.")]
    private static async Task ExecuteAsync(string connectionString, string commandText, CancellationToken ct)
    {
        await using var postgres = new NpgsqlConnection(connectionString);
        await postgres.OpenAsync(ct);
        await using var command = new NpgsqlCommand(commandText, postgres);
        await command.ExecuteNonQueryAsync(ct);
    }
}
