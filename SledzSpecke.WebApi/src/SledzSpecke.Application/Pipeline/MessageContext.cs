using System;
using System.Collections.Generic;

namespace SledzSpecke.Application.Pipeline;

public class MessageContext
{
    public Guid MessageId { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public object Payload { get; set; } = null!;
    public Dictionary<string, object> Headers { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public int RetryCount { get; set; }
    public List<string> ExecutionLog { get; set; } = new();
    public bool IsProcessed { get; set; }
    public string? ErrorMessage { get; set; }
    
    public T GetPayload<T>() => (T)Payload;
    
    public void Log(string message)
    {
        ExecutionLog.Add($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} - {message}");
    }
}