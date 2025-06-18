using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace SledzSpecke.Tests.Common.Reports;

public class TestResultsCollector
{
    private readonly string _outputPath;
    
    public TestResultsCollector(string outputPath = "TestResults")
    {
        _outputPath = outputPath;
        Directory.CreateDirectory(_outputPath);
    }
    
    public void CollectResults(string testRunName)
    {
        var timestamp = DateTime.UtcNow;
        var results = new TestRunResults
        {
            Name = testRunName,
            Timestamp = timestamp,
            MachineName = Environment.MachineName,
            Results = CollectAllResults()
        };
        
        // Save as JSON
        var json = JsonSerializer.Serialize(results, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        File.WriteAllText(Path.Combine(_outputPath, $"testrun_{timestamp:yyyyMMdd_HHmmss}.json"), json);
        
        // Generate HTML report
        GenerateHtmlReport(results);
    }
    
    private List<TestResult> CollectAllResults()
    {
        var results = new List<TestResult>();
        
        // Collect from TRX files
        foreach (var trxFile in Directory.GetFiles(".", "*.trx", SearchOption.AllDirectories))
        {
            results.AddRange(ParseTrxFile(trxFile));
        }
        
        return results;
    }
    
    private IEnumerable<TestResult> ParseTrxFile(string trxPath)
    {
        var doc = XDocument.Load(trxPath);
        var ns = doc.Root?.GetDefaultNamespace();
        
        if (ns == null) yield break;
        
        foreach (var test in doc.Descendants(ns + "UnitTestResult"))
        {
            yield return new TestResult
            {
                Name = test.Attribute("testName")?.Value ?? "Unknown",
                Outcome = test.Attribute("outcome")?.Value ?? "Unknown",
                Duration = TimeSpan.Parse(test.Attribute("duration")?.Value ?? "0"),
                ErrorMessage = test.Element(ns + "Output")?.Element(ns + "ErrorInfo")?.Element(ns + "Message")?.Value,
                StackTrace = test.Element(ns + "Output")?.Element(ns + "ErrorInfo")?.Element(ns + "StackTrace")?.Value
            };
        }
    }
    
    private void GenerateHtmlReport(TestRunResults results)
    {
        var html = $@"
<!DOCTYPE html>
<html lang='pl'>
<head>
    <meta charset='UTF-8'>
    <title>Test Results - {results.Name}</title>
    <style>
        body {{ 
            font-family: 'Segoe UI', Arial, sans-serif; 
            margin: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        h1 {{ 
            color: #333;
            border-bottom: 3px solid #4CAF50;
            padding-bottom: 10px;
        }}
        .summary {{ 
            background: #f0f0f0; 
            padding: 15px; 
            border-radius: 5px;
            margin-bottom: 20px;
        }}
        .stats {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin: 20px 0;
        }}
        .stat-box {{
            background: #fff;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 15px;
            text-align: center;
        }}
        .stat-value {{
            font-size: 2em;
            font-weight: bold;
            margin: 5px 0;
        }}
        .passed {{ color: #4CAF50; }}
        .failed {{ color: #f44336; }}
        .skipped {{ color: #ff9800; }}
        table {{ 
            width: 100%; 
            border-collapse: collapse; 
            margin-top: 20px; 
        }}
        th, td {{ 
            border: 1px solid #ddd; 
            padding: 12px; 
            text-align: left; 
        }}
        th {{ 
            background-color: #4CAF50; 
            color: white;
            position: sticky;
            top: 0;
        }}
        tr:nth-child(even) {{ background-color: #f9f9f9; }}
        tr:hover {{ background-color: #f5f5f5; }}
        .error-details {{
            background: #ffebee;
            padding: 10px;
            border-radius: 4px;
            margin-top: 5px;
            font-family: monospace;
            font-size: 0.9em;
            white-space: pre-wrap;
        }}
        .filter-container {{
            margin: 20px 0;
        }}
        .filter-input {{
            padding: 8px;
            width: 300px;
            border: 1px solid #ddd;
            border-radius: 4px;
        }}
        .duration-bar {{
            background: #e0e0e0;
            height: 20px;
            border-radius: 3px;
            position: relative;
            overflow: hidden;
        }}
        .duration-fill {{
            background: #4CAF50;
            height: 100%;
            transition: width 0.3s;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>🧪 Test Results: {results.Name}</h1>
        <div class='summary'>
            <p><strong>Run at:</strong> {results.Timestamp:yyyy-MM-dd HH:mm:ss} UTC</p>
            <p><strong>Machine:</strong> {results.MachineName}</p>
            <p><strong>Total Duration:</strong> {results.TotalDuration.TotalSeconds:F1}s</p>
        </div>
        
        <div class='stats'>
            <div class='stat-box'>
                <div class='stat-label'>Total Tests</div>
                <div class='stat-value'>{results.Results.Count}</div>
            </div>
            <div class='stat-box'>
                <div class='stat-label'>Passed</div>
                <div class='stat-value passed'>{results.PassedCount}</div>
            </div>
            <div class='stat-box'>
                <div class='stat-label'>Failed</div>
                <div class='stat-value failed'>{results.FailedCount}</div>
            </div>
            <div class='stat-box'>
                <div class='stat-label'>Success Rate</div>
                <div class='stat-value'>{results.SuccessRate:P1}</div>
            </div>
        </div>
        
        <div class='filter-container'>
            <input type='text' class='filter-input' id='testFilter' placeholder='Filter tests...' onkeyup='filterTests()'>
        </div>
        
        <table id='testTable'>
            <tr>
                <th>Test Name</th>
                <th>Result</th>
                <th>Duration</th>
                <th>Details</th>
            </tr>
            {string.Join("\n", results.Results.OrderBy(r => r.Outcome).ThenBy(r => r.Name).Select(r => $@"
            <tr class='test-row' data-name='{r.Name.ToLower()}' data-outcome='{r.Outcome.ToLower()}'>
                <td>{r.Name}</td>
                <td class='{r.Outcome.ToLower()}'>{r.Outcome}</td>
                <td>
                    <div class='duration-bar'>
                        <div class='duration-fill' style='width: {Math.Min(100, r.Duration.TotalMilliseconds / 1000 * 10)}%'></div>
                    </div>
                    {r.Duration.TotalMilliseconds:F0}ms
                </td>
                <td>
                    {(string.IsNullOrEmpty(r.ErrorMessage) ? "-" : $@"
                    <details>
                        <summary>Error Details</summary>
                        <div class='error-details'>{r.ErrorMessage}</div>
                    </details>")}
                </td>
            </tr>"))}
        </table>
    </div>
    
    <script>
        function filterTests() {{
            const filter = document.getElementById('testFilter').value.toLowerCase();
            const rows = document.querySelectorAll('.test-row');
            
            rows.forEach(row => {{
                const name = row.getAttribute('data-name');
                const outcome = row.getAttribute('data-outcome');
                const match = name.includes(filter) || outcome.includes(filter);
                row.style.display = match ? '' : 'none';
            }});
        }}
    </script>
</body>
</html>";
        
        File.WriteAllText(Path.Combine(_outputPath, $"testrun_{results.Timestamp:yyyyMMdd_HHmmss}.html"), html);
    }
}

public class TestRunResults
{
    public string Name { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string MachineName { get; set; } = "";
    public List<TestResult> Results { get; set; } = new();
    
    public int PassedCount => Results.Count(r => r.Outcome == "Passed");
    public int FailedCount => Results.Count(r => r.Outcome == "Failed");
    public double SuccessRate => Results.Count > 0 ? (double)PassedCount / Results.Count : 0;
    public TimeSpan TotalDuration => TimeSpan.FromMilliseconds(Results.Sum(r => r.Duration.TotalMilliseconds));
}

public class TestResult
{
    public string Name { get; set; } = "";
    public string Outcome { get; set; } = "";
    public TimeSpan Duration { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
}