using Microsoft.AspNetCore.Mvc;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/e2e")]
public class E2EResultsController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<E2EResultsController> _logger;

    public E2EResultsController(IWebHostEnvironment environment, ILogger<E2EResultsController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpGet("results")]
    public IActionResult GetResults()
    {
        try
        {
            var resultsPath = Path.Combine(_environment.ContentRootPath, "e2e-results");
            
            if (!Directory.Exists(resultsPath))
            {
                _logger.LogWarning("E2E results directory not found: {Path}", resultsPath);
                return Ok(Array.Empty<object>());
            }

            var results = new List<object>();
            var resultFiles = Directory.GetFiles(resultsPath, "*.json");

            foreach (var file in resultFiles)
            {
                try
                {
                    var json = System.IO.File.ReadAllText(file);
                    var result = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    
                    // Add video path if exists
                    var videoName = Path.GetFileNameWithoutExtension(file) + ".webm";
                    var videoPath = Path.Combine(resultsPath, videoName);
                    if (System.IO.File.Exists(videoPath))
                    {
                        result["videoPath"] = $"/e2e-results/{videoName}";
                    }

                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading result file: {File}", file);
                }
            }

            return Ok(results.OrderByDescending(r => 
            {
                if (r is Dictionary<string, object> dict && dict.TryGetValue("timestamp", out var timestamp))
                {
                    return timestamp?.ToString() ?? "";
                }
                return "";
            }).Take(20));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting E2E results");
            return StatusCode(500, new { error = "Failed to load results" });
        }
    }
}