namespace SledzSpecke.Core.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Data { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }
    
    protected OutboxMessage() { } // For EF Core
    
    public OutboxMessage(string type, string data, Dictionary<string, object>? metadata = null)
    {
        Id = Guid.NewGuid();
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Data = data ?? throw new ArgumentNullException(nameof(data));
        Metadata = metadata ?? new Dictionary<string, object>();
        OccurredAt = DateTime.UtcNow;
        RetryCount = 0;
    }
    
    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
    }
    
    public void MarkAsFailed(string error)
    {
        Error = error;
        RetryCount++;
    }
    
    public bool ShouldRetry => RetryCount < 3 && ProcessedAt == null;
}