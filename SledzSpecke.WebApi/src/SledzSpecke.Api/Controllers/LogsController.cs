using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SledzSpecke.Api.Controllers;

/// <summary>
/// Provides access to structured logs (development only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LogsController> _logger;
    
    public LogsController(IWebHostEnvironment environment, ILogger<LogsController> logger)
    {
        _environment = environment;
        _logger = logger;
    }
    
    /// <summary>
    /// Gets recent structured logs
    /// </summary>
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentLogs(
        [FromQuery] int count = 100,
        [FromQuery] string? level = null,
        [FromQuery] string? correlationId = null)
    {
        // Temporarily enabled in production - REMOVE BEFORE CUSTOMER RELEASE
        // TODO: Remove this comment and uncomment the check below before going live
        // if (!_environment.IsDevelopment())
        // {
        //     return Forbid("Logs are only available in development environment");
        // }
        
        var logPath = "/var/log/sledzspecke";
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var logFile = Path.Combine(logPath, $"structured-{today}.json");
        
        if (!System.IO.File.Exists(logFile))
        {
            return Ok(new { logs = Array.Empty<object>(), message = "No logs found for today" });
        }
        
        var logs = new List<object>();
        var lines = await System.IO.File.ReadAllLinesAsync(logFile);
        
        foreach (var line in lines.TakeLast(count).Reverse())
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            try
            {
                var log = JsonSerializer.Deserialize<JsonElement>(line);
                
                // Filter by level if specified
                if (!string.IsNullOrEmpty(level))
                {
                    var logLevel = log.GetProperty("Level").GetString();
                    if (!string.Equals(logLevel, level, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                
                // Filter by correlation ID if specified
                if (!string.IsNullOrEmpty(correlationId))
                {
                    if (log.TryGetProperty("CorrelationId", out var corrId))
                    {
                        if (corrId.GetString() != correlationId)
                            continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                
                logs.Add(log);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse log line");
            }
        }
        
        return Ok(new 
        { 
            logs = logs.Take(count),
            count = logs.Count,
            file = logFile,
            filters = new { level, correlationId }
        });
    }
    
    /// <summary>
    /// Gets error logs
    /// </summary>
    [HttpGet("errors")]
    public async Task<IActionResult> GetErrorLogs([FromQuery] int hours = 24)
    {
        // Temporarily enabled in production - REMOVE BEFORE CUSTOMER RELEASE
        // TODO: Remove this comment and uncomment the check below before going live
        // if (!_environment.IsDevelopment())
        // {
        //     return Forbid("Logs are only available in development environment");
        // }
        
        var errors = new List<object>();
        var logPath = "/var/log/sledzspecke";
        var cutoffTime = DateTimeOffset.UtcNow.AddHours(-hours);
        
        // Check today and yesterday's logs
        for (int i = 0; i < 2; i++)
        {
            var date = DateTime.UtcNow.AddDays(-i).ToString("yyyy-MM-dd");
            var logFile = Path.Combine(logPath, $"structured-{date}.json");
            
            if (!System.IO.File.Exists(logFile)) continue;
            
            var lines = await System.IO.File.ReadAllLinesAsync(logFile);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                try
                {
                    var log = JsonSerializer.Deserialize<JsonElement>(line);
                    var level = log.GetProperty("Level").GetString();
                    
                    if (level == "Error" || level == "Fatal")
                    {
                        var timestamp = log.GetProperty("Timestamp").GetDateTimeOffset();
                        if (timestamp >= cutoffTime)
                        {
                            errors.Add(log);
                        }
                    }
                }
                catch { }
            }
        }
        
        return Ok(new 
        { 
            errors = errors.OrderByDescending(e => ((JsonElement)e).GetProperty("Timestamp").GetDateTimeOffset()),
            count = errors.Count,
            since = cutoffTime,
            hours
        });
    }
    
    /// <summary>
    /// Gets monitoring statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] int hours = 24)
    {
        if (!_environment.IsDevelopment())
        {
            return Forbid("Stats are only available in development environment");
        }
        
        var totalRequests = 0;
        var totalErrors = 0;
        var totalWarnings = 0;
        var avgResponseTime = 0;
        var topEndpoints = new List<object>();
        var errorsByType = new Dictionary<string, int>();
        var requestsPerHour = new List<object>();
        
        var logPath = "/var/log/sledzspecke";
        var cutoffTime = DateTimeOffset.UtcNow.AddHours(-hours);
        
        // Analyze logs
        for (int i = 0; i < Math.Min(hours / 24 + 1, 7); i++)
        {
            var date = DateTime.UtcNow.AddDays(-i).ToString("yyyy-MM-dd");
            var logFile = Path.Combine(logPath, $"structured-{date}.json");
            
            if (!System.IO.File.Exists(logFile)) continue;
            
            var lines = await System.IO.File.ReadAllLinesAsync(logFile);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                try
                {
                    var log = JsonSerializer.Deserialize<JsonElement>(line);
                    var timestamp = log.GetProperty("Timestamp").GetDateTimeOffset();
                    
                    if (timestamp < cutoffTime) continue;
                    
                    // Count by level
                    var level = log.GetProperty("Level").GetString();
                    if (level == "Error" || level == "Fatal") totalErrors++;
                    if (level == "Warning") totalWarnings++;
                    
                    // Count requests
                    if (log.TryGetProperty("RequestPath", out var path))
                    {
                        totalRequests++;
                    }
                }
                catch { }
            }
        }
        
        return Ok(new
        {
            totalRequests,
            totalErrors,
            totalWarnings,
            avgResponseTime,
            topEndpoints,
            errorsByType,
            requestsPerHour
        });
    }
}