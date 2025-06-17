using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SledzSpecke.Api.Controllers;

[Route("api/[controller]")]
public class PerformanceMetricsController : Controller
{
    private static readonly ConcurrentDictionary<string, PerformanceMetric> _metrics = new();
    private readonly IMemoryCache _cache;

    public PerformanceMetricsController(IMemoryCache cache)
    {
        _cache = cache;
    }

    [HttpGet("/performance-dashboard")]
    public IActionResult ViewDashboard()
    {
        // Return the HTML directly as a simple solution
        var html = @"<!DOCTYPE html>
<html>
<head>
    <title>Performance Metrics Dashboard</title>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }
        .container {
            max-width: 1400px;
            margin: 0 auto;
        }
        h1 {
            color: #333;
            margin-bottom: 30px;
        }
        .metrics-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 40px;
        }
        .metric-card {
            background: white;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .metric-value {
            font-size: 2.5em;
            font-weight: bold;
            color: #4CAF50;
            margin: 10px 0;
        }
        .metric-label {
            color: #666;
            font-size: 0.9em;
            text-transform: uppercase;
        }
        table {
            width: 100%;
            background: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }
        th {
            background: #4CAF50;
            color: white;
            padding: 12px;
            text-align: left;
        }
        td {
            padding: 12px;
            border-bottom: 1px solid #eee;
        }
        tr:hover {
            background: #f9f9f9;
        }
        .error {
            color: #f44336;
        }
        .success {
            color: #4CAF50;
        }
        .refresh-btn {
            background: #4CAF50;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
        }
        .refresh-btn:hover {
            background: #45a049;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <h1>Performance Metrics Dashboard</h1>
        <button class=""refresh-btn"" onclick=""loadMetrics()"">Refresh</button>
        
        <div class=""metrics-grid"" id=""summaryMetrics"">
            <!-- Summary metrics will be loaded here -->
        </div>

        <h2>Top Operations</h2>
        <table id=""operationsTable"">
            <thead>
                <tr>
                    <th>Operation</th>
                    <th>Total Calls</th>
                    <th>Avg Time (ms)</th>
                    <th>Min Time (ms)</th>
                    <th>Max Time (ms)</th>
                    <th>Success Rate</th>
                    <th>Last Call</th>
                </tr>
            </thead>
            <tbody>
                <!-- Data will be loaded here -->
            </tbody>
        </table>

        <h2>Recent Errors</h2>
        <table id=""errorsTable"">
            <thead>
                <tr>
                    <th>Operation</th>
                    <th>Error Message</th>
                    <th>Timestamp</th>
                </tr>
            </thead>
            <tbody>
                <!-- Data will be loaded here -->
            </tbody>
        </table>
    </div>

    <script>
        async function loadMetrics() {
            try {
                const response = await fetch('/api/performancemetrics/dashboard');
                const data = await response.json();
                
                // Update summary metrics
                const summaryHtml = `
                    <div class=""metric-card"">
                        <div class=""metric-label"">Total Operations</div>
                        <div class=""metric-value"">${data.totalOperations}</div>
                    </div>
                    <div class=""metric-card"">
                        <div class=""metric-label"">Unique Operations</div>
                        <div class=""metric-value"">${data.uniqueOperations}</div>
                    </div>
                    <div class=""metric-card"">
                        <div class=""metric-label"">Success Rate</div>
                        <div class=""metric-value"">${data.overallSuccessRate.toFixed(1)}%</div>
                    </div>
                    <div class=""metric-card"">
                        <div class=""metric-label"">Avg Response Time</div>
                        <div class=""metric-value"">${data.averageResponseTime.toFixed(0)}ms</div>
                    </div>
                    <div class=""metric-card"">
                        <div class=""metric-label"">Cache Hit Rate</div>
                        <div class=""metric-value"">${data.cacheStatistics?.cacheHitRate?.toFixed(1) || 0}%</div>
                    </div>
                `;
                document.getElementById('summaryMetrics').innerHTML = summaryHtml;
                
                // Update operations table
                const operationsBody = document.querySelector('#operationsTable tbody');
                operationsBody.innerHTML = data.topOperations.map(op => `
                    <tr>
                        <td>${op.operation}</td>
                        <td>${op.totalCalls}</td>
                        <td>${op.averageMs.toFixed(2)}</td>
                        <td>${op.minMs.toFixed(2)}</td>
                        <td>${op.maxMs.toFixed(2)}</td>
                        <td class=""${op.successRate >= 95 ? 'success' : 'error'}"">${op.successRate.toFixed(1)}%</td>
                        <td>${new Date(op.lastCall).toLocaleString()}</td>
                    </tr>
                `).join('');
                
                // Update errors table
                const errorsBody = document.querySelector('#errorsTable tbody');
                errorsBody.innerHTML = data.recentErrors.map(err => `
                    <tr>
                        <td>${err.operationName}</td>
                        <td class=""error"">${err.errorMessage || 'Unknown error'}</td>
                        <td>${new Date(err.timestamp).toLocaleString()}</td>
                    </tr>
                `).join('');
                
            } catch (error) {
                console.error('Failed to load metrics:', error);
            }
        }
        
        // Load metrics on page load
        loadMetrics();
        
        // Auto-refresh every 10 seconds
        setInterval(loadMetrics, 10000);
    </script>
</body>
</html>";
        
        return Content(html, "text/html");
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        var metrics = _metrics.Values
            .GroupBy(m => m.OperationName)
            .Select(g => new
            {
                Operation = g.Key,
                TotalCalls = g.Count(),
                AverageMs = g.Average(m => m.ElapsedMilliseconds),
                MinMs = g.Min(m => m.ElapsedMilliseconds),
                MaxMs = g.Max(m => m.ElapsedMilliseconds),
                SuccessRate = (g.Count(m => m.IsSuccess) / (double)g.Count()) * 100,
                LastCall = g.Max(m => m.Timestamp),
                Errors = g.Where(m => !m.IsSuccess).Select(m => new { m.ErrorMessage, m.Timestamp }).Take(5)
            })
            .OrderByDescending(m => m.TotalCalls)
            .ToList();

        var cacheStats = new
        {
            CacheHits = 0, // GetCurrentStatistics is not available in standard IMemoryCache
            CacheMisses = 0,
            CacheHitRate = 0.0
        };

        var summary = new
        {
            TotalOperations = _metrics.Count,
            UniqueOperations = metrics.Count,
            OverallSuccessRate = _metrics.Values.Any() 
                ? (_metrics.Values.Count(m => m.IsSuccess) / (double)_metrics.Count) * 100 
                : 100,
            AverageResponseTime = _metrics.Values.Any() 
                ? _metrics.Values.Average(m => m.ElapsedMilliseconds) 
                : 0,
            CacheStatistics = cacheStats,
            TopOperations = metrics.Take(10),
            RecentErrors = _metrics.Values
                .Where(m => !m.IsSuccess)
                .OrderByDescending(m => m.Timestamp)
                .Take(10)
                .Select(m => new { m.OperationName, m.ErrorMessage, m.Timestamp })
        };

        return Ok(summary);
    }

    [HttpPost("record")]
    public IActionResult RecordMetric([FromBody] PerformanceMetric metric)
    {
        if (metric == null)
            return BadRequest();

        metric.Id = Guid.NewGuid().ToString();
        metric.Timestamp = DateTime.UtcNow;
        
        _metrics.TryAdd(metric.Id, metric);

        // Keep only last 1000 metrics to prevent memory issues
        if (_metrics.Count > 1000)
        {
            var oldestKey = _metrics.OrderBy(kvp => kvp.Value.Timestamp).First().Key;
            _metrics.TryRemove(oldestKey, out _);
        }

        return Ok();
    }

    [HttpDelete("clear")]
    public IActionResult ClearMetrics()
    {
        _metrics.Clear();
        return Ok(new { message = "All metrics cleared" });
    }

    public class PerformanceMetric
    {
        public string Id { get; set; } = string.Empty;
        public string OperationName { get; set; } = string.Empty;
        public long ElapsedMilliseconds { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object>? AdditionalData { get; set; }
    }
}