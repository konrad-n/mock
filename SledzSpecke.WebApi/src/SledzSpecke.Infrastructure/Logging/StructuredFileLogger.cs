using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SledzSpecke.Infrastructure.Logging;

/// <summary>
/// Custom structured file logger that provides Seq-like functionality
/// </summary>
public class StructuredFileLogger
{
    private readonly string _logPath;
    private readonly object _lock = new();
    
    public StructuredFileLogger(IOptions<LoggingSettings> options)
    {
        _logPath = options.Value.StructuredLogPath ?? "/var/log/sledzspecke/structured";
        Directory.CreateDirectory(_logPath);
    }
    
    public async Task LogEventAsync(LogEvent logEvent)
    {
        var fileName = Path.Combine(_logPath, $"log-{DateTime.UtcNow:yyyy-MM-dd}.json");
        var json = JsonSerializer.Serialize(logEvent, new JsonSerializerOptions
        {
            WriteIndented = false
        });
        
        lock (_lock)
        {
            File.AppendAllText(fileName, json + Environment.NewLine);
        }
        
        await Task.CompletedTask;
    }
}

public class LogEvent
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string Level { get; set; } = "Information";
    public string MessageTemplate { get; set; } = string.Empty;
    public Dictionary<string, object?> Properties { get; set; } = new();
    public string? Exception { get; set; }
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public string? RemoteIp { get; set; }
}

public class LoggingSettings
{
    public string StructuredLogPath { get; set; } = "/var/log/sledzspecke/structured";
    public bool EnableStructuredLogging { get; set; } = true;
    public int RetentionDays { get; set; } = 7;
}