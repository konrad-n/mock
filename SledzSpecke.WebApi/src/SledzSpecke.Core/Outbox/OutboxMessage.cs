namespace SledzSpecke.Core.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Data { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }
    public Dictionary<string, object>? Metadata { get; private set; }

    public OutboxMessage(string type, string data, Dictionary<string, object>? metadata = null)
    {
        Id = Guid.NewGuid();
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Data = data ?? throw new ArgumentNullException(nameof(data));
        OccurredAt = DateTime.UtcNow;
        RetryCount = 0;
        Metadata = metadata;
    }

    // EF Core constructor
    private OutboxMessage() { }

    public void MarkAsProcessed()
    {
        if (ProcessedAt.HasValue)
            throw new InvalidOperationException("Message already processed");

        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
        RetryCount++;
    }

    public bool ShouldRetry(int maxRetries = 3)
    {
        return !ProcessedAt.HasValue && RetryCount < maxRetries;
    }
}