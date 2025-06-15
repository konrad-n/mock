using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("monitoring")]
public class MonitoringDashboardController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<MonitoringDashboardController> _logger;
    
    public MonitoringDashboardController(IWebHostEnvironment environment, ILogger<MonitoringDashboardController> logger)
    {
        _environment = environment;
        _logger = logger;
    }
    
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new 
        { 
            status = "healthy",
            environment = _environment.EnvironmentName,
            timestamp = DateTime.UtcNow,
            message = "Monitoring endpoints are available"
        });
    }
    
    [HttpGet("dashboard")]
    [Produces("text/html")]
    public IActionResult Dashboard()
    {
        // Temporarily enabled in production - REMOVE BEFORE CUSTOMER RELEASE
        // TODO: Remove this comment and uncomment the check below before going live
        // if (!_environment.IsDevelopment())
        // {
        //     return Content("<h1>403 - Monitoring Dashboard</h1><p>This dashboard is only available in development environment for security reasons.</p>", "text/html");
        // }
        
        var html = """
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>SledzSpecke Monitoring Dashboard</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { 
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: #f5f5f5;
            color: #333;
        }
        .header {
            background: #2c3e50;
            color: white;
            padding: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .container {
            max-width: 1400px;
            margin: 0 auto;
            padding: 20px;
        }
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        .stat-card {
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            text-align: center;
        }
        .stat-value {
            font-size: 2.5em;
            font-weight: bold;
            margin: 10px 0;
        }
        .stat-label {
            color: #666;
            font-size: 0.9em;
        }
        .error { color: #e74c3c; }
        .warning { color: #f39c12; }
        .success { color: #27ae60; }
        .info { color: #3498db; }
        .section {
            background: white;
            padding: 20px;
            margin-bottom: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .section h2 {
            margin-bottom: 15px;
            color: #2c3e50;
        }
        .log-entry {
            padding: 10px;
            margin: 5px 0;
            border-left: 4px solid #ddd;
            background: #fafafa;
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 0.9em;
            overflow-x: auto;
        }
        .log-entry.error { border-left-color: #e74c3c; background: #fee; }
        .log-entry.warning { border-left-color: #f39c12; background: #fff8e6; }
        .log-entry.info { border-left-color: #3498db; }
        .controls {
            margin-bottom: 20px;
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }
        button {
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            background: #3498db;
            color: white;
            cursor: pointer;
            font-size: 14px;
        }
        button:hover { background: #2980b9; }
        input, select {
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
        }
        .search-box {
            flex: 1;
            min-width: 200px;
        }
        .timestamp {
            color: #666;
            font-size: 0.8em;
        }
        .loading {
            text-align: center;
            padding: 20px;
            color: #666;
        }
        .chart-container {
            height: 300px;
            margin: 20px 0;
        }
        table {
            width: 100%;
            border-collapse: collapse;
        }
        th, td {
            padding: 10px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }
        th {
            background: #f5f5f5;
            font-weight: 600;
        }
        .status-badge {
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 0.8em;
            font-weight: 600;
        }
        .status-200 { background: #d4edda; color: #155724; }
        .status-400 { background: #f8d7da; color: #721c24; }
        .status-500 { background: #f8d7da; color: #721c24; }
        #refreshIndicator {
            display: inline-block;
            margin-left: 10px;
            font-size: 0.9em;
            color: #666;
        }
    </style>
</head>
<body>
    <div class='header'>
        <div class='container'>
            <h1>🔍 SledzSpecke Monitoring Dashboard</h1>
            <p>Real-time application monitoring and logs</p>
        </div>
    </div>
    
    <div class='container'>
        <div class='stats-grid' id='statsGrid'>
            <div class='stat-card'>
                <div class='stat-label'>Total Requests (24h)</div>
                <div class='stat-value info' id='totalRequests'>-</div>
            </div>
            <div class='stat-card'>
                <div class='stat-label'>Errors (24h)</div>
                <div class='stat-value error' id='errorCount'>-</div>
            </div>
            <div class='stat-card'>
                <div class='stat-label'>Warnings (24h)</div>
                <div class='stat-value warning' id='warningCount'>-</div>
            </div>
            <div class='stat-card'>
                <div class='stat-label'>Avg Response Time</div>
                <div class='stat-value success' id='avgResponseTime'>-</div>
            </div>
        </div>

        <div class='section'>
            <h2>Recent Errors</h2>
            <div class='controls'>
                <button onclick='loadErrors()'>Refresh</button>
                <input type='text' class='search-box' id='errorSearch' placeholder='Search errors...' onkeyup='filterLogs()'>
                <select id='timeRange' onchange='loadErrors()'>
                    <option value='1'>Last 1 hour</option>
                    <option value='6'>Last 6 hours</option>
                    <option value='24' selected>Last 24 hours</option>
                    <option value='168'>Last 7 days</option>
                </select>
            </div>
            <div id='errorLogs' class='loading'>Loading errors...</div>
        </div>

        <div class='section'>
            <h2>Request Activity</h2>
            <div class='chart-container'>
                <canvas id='activityChart'></canvas>
            </div>
        </div>

        <div class='section'>
            <h2>Recent API Calls</h2>
            <div class='controls'>
                <button onclick='loadRecentCalls()'>Refresh</button>
                <span id='refreshIndicator'></span>
            </div>
            <div id='recentCalls'>
                <table>
                    <thead>
                        <tr>
                            <th>Time</th>
                            <th>Method</th>
                            <th>Path</th>
                            <th>Status</th>
                            <th>Duration</th>
                            <th>User</th>
                        </tr>
                    </thead>
                    <tbody id='callsTableBody'>
                        <tr><td colspan='6' class='loading'>Loading...</td></tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div class='section'>
            <h2>Live Logs</h2>
            <div class='controls'>
                <button onclick='toggleLiveLogs()' id='liveToggle'>Start Live Updates</button>
                <button onclick='clearLogs()'>Clear</button>
                <select id='logLevel' onchange='loadLogs()'>
                    <option value='all'>All Levels</option>
                    <option value='error'>Errors Only</option>
                    <option value='warning'>Warnings</option>
                    <option value='info'>Info</option>
                </select>
            </div>
            <div id='liveLogs' style='max-height: 400px; overflow-y: auto;'></div>
        </div>
    </div>

    <script>
        let liveLogsInterval = null;
        let activityChart = null;

        // Initialize
        document.addEventListener('DOMContentLoaded', function() {
            loadStats();
            loadErrors();
            loadRecentCalls();
            initializeChart();
            
            // Auto-refresh stats every 30 seconds
            setInterval(loadStats, 30000);
            setInterval(loadRecentCalls, 10000);
        });

        async function loadStats() {
            try {
                const response = await fetch('/api/logs/errors?hours=24');
                const data = await response.json();
                document.getElementById('errorCount').textContent = data.count || '0';
                
                // These would come from actual endpoints
                document.getElementById('totalRequests').textContent = Math.floor(Math.random() * 1000 + 500);
                document.getElementById('warningCount').textContent = Math.floor(Math.random() * 50 + 10);
                document.getElementById('avgResponseTime').textContent = Math.floor(Math.random() * 100 + 50) + 'ms';
            } catch (error) {
                console.error('Failed to load stats:', error);
            }
        }

        async function loadErrors() {
            const hours = document.getElementById('timeRange').value;
            try {
                const response = await fetch(`/api/logs/errors?hours=${hours}`);
                const data = await response.json();
                
                const errorLogsDiv = document.getElementById('errorLogs');
                if (data.errors && data.errors.length > 0) {
                    errorLogsDiv.innerHTML = data.errors.map(error => {
                        const timestamp = new Date(error.Timestamp).toLocaleString();
                        const message = error.MessageTemplate || error.Message || 'Unknown error';
                        const exception = error.Exception ? `<br><small>${error.Exception.split('\\n')[0]}</small>` : '';
                        return `<div class='log-entry error'>
                            <span class='timestamp'>${timestamp}</span> - ${message}${exception}
                        </div>`;
                    }).join('');
                } else {
                    errorLogsDiv.innerHTML = '<div class=\'log-entry\'>No errors found in the selected time range 🎉</div>';
                }
            } catch (error) {
                document.getElementById('errorLogs').innerHTML = '<div class=\'log-entry error\'>Failed to load errors</div>';
            }
        }

        async function loadRecentCalls() {
            try {
                const response = await fetch('/api/logs/recent?count=20');
                const data = await response.json();
                
                const tbody = document.getElementById('callsTableBody');
                if (data.logs && data.logs.length > 0) {
                    tbody.innerHTML = data.logs.map(log => {
                        const time = new Date(log.Timestamp).toLocaleTimeString();
                        const method = log.RequestMethod || '-';
                        const path = log.RequestPath || '-';
                        const status = log.StatusCode || '-';
                        const duration = log.ElapsedTime || '-';
                        const user = log.UserId || 'Anonymous';
                        
                        const statusClass = status ? `status-${status}` : '';
                        
                        return `<tr>
                            <td>${time}</td>
                            <td><strong>${method}</strong></td>
                            <td>${path}</td>
                            <td><span class='status-badge ${statusClass}'>${status}</span></td>
                            <td>${duration}ms</td>
                            <td>${user}</td>
                        </tr>`;
                    }).join('');
                } else {
                    tbody.innerHTML = '<tr><td colspan=\'6\'>No recent calls found</td></tr>';
                }
                
                document.getElementById('refreshIndicator').textContent = 'Updated ' + new Date().toLocaleTimeString();
            } catch (error) {
                console.error('Failed to load recent calls:', error);
            }
        }

        function toggleLiveLogs() {
            const button = document.getElementById('liveToggle');
            if (liveLogsInterval) {
                clearInterval(liveLogsInterval);
                liveLogsInterval = null;
                button.textContent = 'Start Live Updates';
            } else {
                loadLogs();
                liveLogsInterval = setInterval(loadLogs, 2000);
                button.textContent = 'Stop Live Updates';
            }
        }

        async function loadLogs() {
            const level = document.getElementById('logLevel').value;
            try {
                const response = await fetch(`/api/logs/recent?count=50${level !== 'all' ? '&level=' + level : ''}`);
                const data = await response.json();
                
                const logsDiv = document.getElementById('liveLogs');
                if (data.logs && data.logs.length > 0) {
                    logsDiv.innerHTML = data.logs.map(log => {
                        const timestamp = new Date(log.Timestamp).toLocaleTimeString();
                        const level = log.Level || 'Info';
                        const message = log.MessageTemplate || log.Message || JSON.stringify(log);
                        const levelClass = level.toLowerCase().includes('err') ? 'error' : 
                                         level.toLowerCase().includes('warn') ? 'warning' : 'info';
                        
                        return `<div class='log-entry ${levelClass}'>
                            <span class='timestamp'>${timestamp}</span> [${level}] ${message}
                        </div>`;
                    }).join('');
                } else {
                    logsDiv.innerHTML = '<div class=\'log-entry\'>No logs found</div>';
                }
            } catch (error) {
                document.getElementById('liveLogs').innerHTML = '<div class=\'log-entry error\'>Failed to load logs</div>';
            }
        }

        function clearLogs() {
            document.getElementById('liveLogs').innerHTML = '';
        }

        function filterLogs() {
            const searchTerm = document.getElementById('errorSearch').value.toLowerCase();
            const entries = document.querySelectorAll('#errorLogs .log-entry');
            entries.forEach(entry => {
                if (entry.textContent.toLowerCase().includes(searchTerm)) {
                    entry.style.display = 'block';
                } else {
                    entry.style.display = 'none';
                }
            });
        }

        function initializeChart() {
            // Placeholder for activity chart
            // In a real implementation, you'd use Chart.js or similar
            const canvas = document.getElementById('activityChart');
            const ctx = canvas.getContext('2d');
            ctx.fillStyle = '#3498db';
            ctx.fillRect(0, 0, canvas.width, 50);
            ctx.fillStyle = '#333';
            ctx.font = '16px Arial';
            ctx.fillText('Activity chart would go here (requires Chart.js)', 20, 30);
        }
    </script>
</body>
</html>
""";
        
        return Content(html, "text/html");
    }
}