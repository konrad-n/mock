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
            var resultsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "e2e-results");
            
            if (!Directory.Exists(resultsPath))
            {
                _logger.LogWarning("E2E results directory not found: {Path}", resultsPath);
                return Ok(Array.Empty<object>());
            }

            var results = new List<object>();
            
            // Check for latest symlink first
            var latestPath = Path.Combine(resultsPath, "latest");
            if (Directory.Exists(latestPath))
            {
                var metadataPath = Path.Combine(latestPath, "metadata.json");
                if (System.IO.File.Exists(metadataPath))
                {
                    try
                    {
                        var json = System.IO.File.ReadAllText(metadataPath);
                        var metadata = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                        
                        // Add video information for each browser
                        var browsers = new[] { "chromium", "firefox" };
                        foreach (var browser in browsers)
                        {
                            var browserVideoPath = Path.Combine(latestPath, browser, "videos");
                            if (Directory.Exists(browserVideoPath))
                            {
                                var videos = Directory.GetFiles(browserVideoPath, "*.webm");
                                if (videos.Length > 0)
                                {
                                    metadata[$"{browser}Videos"] = videos.Select(v => $"/e2e-results/latest/{browser}/videos/{Path.GetFileName(v)}").ToArray();
                                }
                            }
                            
                            var browserScreenshotPath = Path.Combine(latestPath, browser, "screenshots");
                            if (Directory.Exists(browserScreenshotPath))
                            {
                                var screenshots = Directory.GetFiles(browserScreenshotPath, "*.png");
                                if (screenshots.Length > 0)
                                {
                                    metadata[$"{browser}Screenshots"] = screenshots.Select(s => $"/e2e-results/latest/{browser}/screenshots/{Path.GetFileName(s)}").ToArray();
                                }
                            }
                        }
                        
                        results.Add(metadata);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error reading latest metadata");
                    }
                }
            }
            
            // Also get all run directories
            var runDirs = Directory.GetDirectories(resultsPath)
                .Where(d => long.TryParse(Path.GetFileName(d), out _))
                .OrderByDescending(d => Path.GetFileName(d))
                .Take(10);
                
            foreach (var runDir in runDirs)
            {
                var metadataPath = Path.Combine(runDir, "metadata.json");
                if (System.IO.File.Exists(metadataPath))
                {
                    try
                    {
                        var json = System.IO.File.ReadAllText(metadataPath);
                        var metadata = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                        metadata["runPath"] = $"/e2e-results/{Path.GetFileName(runDir)}/";
                        results.Add(metadata);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error reading metadata for run: {RunDir}", runDir);
                    }
                }
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting E2E results");
            return StatusCode(500, new { error = "Failed to load results" });
        }
    }
}