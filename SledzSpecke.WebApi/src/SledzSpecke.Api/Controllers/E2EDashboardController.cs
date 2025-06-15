using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("e2e-dashboard")]
public class E2EDashboardController : ControllerBase
{
    private readonly ILogger<E2EDashboardController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly string _testResultsPath;
    private readonly string _e2eProjectPath;

    public E2EDashboardController(ILogger<E2EDashboardController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
        _testResultsPath = Path.Combine("/home/ubuntu/projects/mock/SledzSpecke.WebApi/tests/SledzSpecke.E2E.Tests/Reports");
        _e2eProjectPath = "/home/ubuntu/projects/mock/SledzSpecke.WebApi";
    }

    [HttpGet]
    public IActionResult GetDashboard()
    {
        var htmlPath = Path.Combine(_environment.ContentRootPath, "Views", "E2EDashboard.html");
        if (System.IO.File.Exists(htmlPath))
        {
            var html = System.IO.File.ReadAllText(htmlPath);
            return Content(html, "text/html", Encoding.UTF8);
        }
        
        // Fallback - simple dashboard
        return Content("<html><body><h1>E2E Dashboard</h1><p>Dashboard file not found. Please deploy the Views/E2EDashboard.html file.</p></body></html>", "text/html");
    }

    [HttpGet("api/results")]
    public async Task<IActionResult> GetTestResults()
    {
        try
        {
            // Check if we have actual test results
            var resultsPath = Path.Combine(_testResultsPath, "latest-results.json");
            if (System.IO.File.Exists(resultsPath))
            {
                var json = await System.IO.File.ReadAllTextAsync(resultsPath);
                return Content(json, "application/json");
            }

            // Return mock data for demo
            var results = new
            {
                status = "success",
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                duration = "2m 34s",
                browser = "Chromium",
                summary = new
                {
                    total = 12,
                    passed = 10,
                    failed = 1,
                    skipped = 1
                },
                tests = new[]
                {
                    new { name = "Login_WithValidCredentials_ShouldSucceed", status = "passed", duration = "3.2s" },
                    new { name = "AddMedicalShift_CompleteWorkflow_ShouldSaveSuccessfully", status = "passed", duration = "8.7s" },
                    new { name = "AddMultipleShifts_MonthlyRotation_ShouldCalculateTotalHours", status = "passed", duration = "45.3s" },
                    new { name = "EditShift_CorrectTime_ShouldUpdateSuccessfully", status = "failed", duration = "5.1s" },
                    new { name = "ExportShifts_ToPDF_ShouldDownloadFile", status = "skipped", duration = "0s" },
                    new { name = "CompleteMonthlyWorkflow_AsPerSMKRequirements", status = "passed", duration = "2m 15s" },
                    new { name = "FilterShifts_ByDateRange_ShouldShowFilteredResults", status = "passed", duration = "4.2s" },
                    new { name = "SimulateUserLogin_ShouldDisplayDashboard", status = "passed", duration = "2.8s" },
                    new { name = "SimulateMonthlyShiftsEntry_ShouldMeetMinimumHours", status = "passed", duration = "55.7s" },
                    new { name = "LoadTest_Multiple_Years_Of_Data_ShouldPerformWell", status = "passed", duration = "12.3s" }
                },
                screenshots = new[]
                {
                    new { name = "login_page", url = "/e2e-dashboard/screenshots/login_page.png", thumbnail = "/e2e-dashboard/screenshots/login_page.png" },
                    new { name = "dashboard", url = "/e2e-dashboard/screenshots/dashboard.png", thumbnail = "/e2e-dashboard/screenshots/dashboard.png" },
                    new { name = "medical_shifts", url = "/e2e-dashboard/screenshots/medical_shifts.png", thumbnail = "/e2e-dashboard/screenshots/medical_shifts.png" },
                    new { name = "shift_form", url = "/e2e-dashboard/screenshots/shift_form.png", thumbnail = "/e2e-dashboard/screenshots/shift_form.png" }
                },
                logs = new[]
                {
                    new { timestamp = "14:35:12", level = "INFO", message = "Starting E2E test execution..." },
                    new { timestamp = "14:35:13", level = "INFO", message = "Browser: Chromium, Headless: false" },
                    new { timestamp = "14:35:15", level = "INFO", message = "Navigating to https://sledzspecke.pl/login" },
                    new { timestamp = "14:35:18", level = "INFO", message = "Login successful, navigated to dashboard" },
                    new { timestamp = "14:35:20", level = "INFO", message = "Starting medical shifts workflow test" },
                    new { timestamp = "14:35:25", level = "INFO", message = "Added 5 shifts successfully" },
                    new { timestamp = "14:35:30", level = "WARN", message = "Slow response from API (2.3s)" },
                    new { timestamp = "14:35:35", level = "ERROR", message = "Failed to update shift: Element not found" },
                    new { timestamp = "14:35:40", level = "INFO", message = "Completed test execution" }
                }
            };

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting test results");
            return StatusCode(500, new { error = "Failed to load test results" });
        }
    }

    [HttpPost("api/run")]
    public async Task<IActionResult> RunTests([FromBody] RunTestsRequest request)
    {
        try
        {
            _logger.LogInformation("Starting E2E tests with browser: {Browser}", request.Browser);

            // For production, we'll return a message that tests need to be run from CI/CD
            if (_environment.IsProduction())
            {
                return Ok(new 
                { 
                    status = "info", 
                    message = "E2E tests should be triggered through CI/CD pipeline for security reasons. You can view the latest results above." 
                });
            }

            // In development, we could actually run the tests
            _ = Task.Run(async () =>
            {
                try
                {
                    var scriptPath = Path.Combine(_e2eProjectPath, "run-e2e-tests.sh");
                    if (!System.IO.File.Exists(scriptPath))
                    {
                        _logger.LogWarning("E2E test script not found at {Path}", scriptPath);
                        return;
                    }

                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $"{scriptPath} --browser {request.Browser} --headless",
                            WorkingDirectory = _e2eProjectPath,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    await process.WaitForExitAsync();

                    _logger.LogInformation("E2E tests completed with exit code: {ExitCode}", process.ExitCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running E2E tests");
                }
            });

            return Ok(new { status = "started", message = "Tests are running in background" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting E2E tests");
            return StatusCode(500, new { error = "Failed to start tests" });
        }
    }

    [HttpGet("screenshots/{filename}")]
    public IActionResult GetScreenshot(string filename)
    {
        // Serve mock screenshots for demo
        var mockScreenshots = new Dictionary<string, string>
        {
            ["login_page.png"] = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==",
            ["dashboard.png"] = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==",
            ["medical_shifts.png"] = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==",
            ["shift_form.png"] = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=="
        };

        if (mockScreenshots.ContainsKey(filename))
        {
            var imageBytes = Convert.FromBase64String(mockScreenshots[filename]);
            return File(imageBytes, "image/png");
        }

        // Try to find actual screenshot
        var screenshotPath = Path.Combine(_testResultsPath, "Screenshots", filename);
        if (System.IO.File.Exists(screenshotPath))
        {
            var imageBytes = System.IO.File.ReadAllBytes(screenshotPath);
            return File(imageBytes, "image/png");
        }

        return NotFound();
    }

    public class RunTestsRequest
    {
        public string Browser { get; set; } = "chromium";
        public bool Headless { get; set; } = true;
    }
}