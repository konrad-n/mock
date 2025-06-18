using System;
using System.Text.Json;

namespace SledzSpecke.Core.Auditing;

public interface IAuditLog
{
    Guid Id { get; }
    string EntityType { get; }
    string EntityId { get; }
    string Action { get; }
    string UserId { get; }
    DateTime Timestamp { get; }
    string? OldValues { get; }
    string? NewValues { get; }
    string? PropertyName { get; }
}

public class AuditLog : IAuditLog
{
    public Guid Id { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? PropertyName { get; private set; }
    
    private AuditLog() { } // For EF
    
    public static AuditLog Create(string entityType, string entityId, string action, 
        string userId, object? oldValues = null, object? newValues = null, string? propertyName = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            PropertyName = propertyName
        };
    }
}