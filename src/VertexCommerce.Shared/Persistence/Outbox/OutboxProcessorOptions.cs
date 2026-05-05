namespace VertexCommerce.Shared.Persistence.Outbox;

public sealed class OutboxProcessorOptions
{
    public string ModuleName { get; set; } = string.Empty;
    public int BatchSize { get; set; } = 20;
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(1);
}
