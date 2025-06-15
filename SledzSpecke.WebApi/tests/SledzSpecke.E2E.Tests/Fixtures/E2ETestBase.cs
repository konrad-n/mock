using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Serilog;
using SledzSpecke.E2E.Tests.Core;
using Xunit;

namespace SledzSpecke.E2E.Tests.Fixtures;

/// <summary>
/// Base class for all E2E tests following Open/Closed Principle
/// </summary>
public abstract class E2ETestBase : IAsyncLifetime
{
    protected IBrowserFactory BrowserFactory { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected TestConfiguration Configuration { get; private set; } = null!;
    protected ILogger Logger { get; private set; } = null!;
    
    private string? _traceFilePath;

    public async Task InitializeAsync()
    {
        // Setup configuration
        Configuration = LoadConfiguration();
        
        // Setup logging
        Logger = ConfigureLogging();
        
        Logger.Information("Initializing E2E test: {TestName}", GetType().Name);
        
        // Create browser factory
        BrowserFactory = new BrowserFactory(Configuration, Logger);
        
        // Create browser context and page
        Context = await BrowserFactory.CreateBrowserContextAsync();
        Page = await Context.NewPageAsync();
        
        // Create necessary directories
        CreateTestDirectories();
        
        // Custom initialization for derived classes
        await OnInitializeAsync();
    }

    public async Task DisposeAsync()
    {
        Logger.Information("Disposing E2E test: {TestName}", GetType().Name);
        
        try
        {
            // Custom cleanup for derived classes
            await OnDisposeAsync();
            
            // Save trace if enabled
            if (Configuration.TraceEnabled && Context != null)
            {
                _traceFilePath = Path.Combine(
                    Configuration.TracePath,
                    $"{GetType().Name}_{DateTime.Now:yyyyMMdd_HHmmss}.zip"
                );
                
                await Context.Tracing.StopAsync(new TracingStopOptions
                {
                    Path = _traceFilePath
                });
                
                Logger.Information("Trace saved: {TracePath}", _traceFilePath);
            }
            
            // Close context
            if (Context != null)
            {
                await Context.CloseAsync();
                
                // Log video files created
                if (Configuration.RecordVideo && Directory.Exists(Configuration.VideoPath))
                {
                    var videos = Directory.GetFiles(Configuration.VideoPath, "*.webm");
                    Logger.Information("Videos created: {Count}", videos.Length);
                    foreach (var video in videos)
                    {
                        Logger.Information("Video file: {Path} ({Size} bytes)", video, new FileInfo(video).Length);
                    }
                }
            }
            
            // Dispose browser factory
            if (BrowserFactory != null)
            {
                await BrowserFactory.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error during test disposal");
        }
    }

    /// <summary>
    /// Override this method to perform custom initialization
    /// </summary>
    protected virtual Task OnInitializeAsync() => Task.CompletedTask;

    /// <summary>
    /// Override this method to perform custom cleanup
    /// </summary>
    protected virtual Task OnDisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// Helper method to take screenshot on test failure
    /// </summary>
    protected async Task TakeFailureScreenshotAsync(string testName)
    {
        try
        {
            var screenshotPath = Path.Combine(
                Configuration.ScreenshotPath,
                $"FAILURE_{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            );
            
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            
            Logger.Error("Failure screenshot saved: {Path}", screenshotPath);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to take failure screenshot");
        }
    }

    /// <summary>
    /// Helper method to simulate real user typing
    /// </summary>
    protected async Task TypeLikeUserAsync(string selector, string text)
    {
        await Page.ClickAsync(selector);
        
        if (Configuration.SmkSimulation.SimulateRealUserSpeed)
        {
            await Page.FillAsync(selector, text);
            await Page.PressAsync(selector, "End"); // Simulate real typing by moving cursor
        }
        else
        {
            await Page.FillAsync(selector, text);
        }
    }

    /// <summary>
    /// Helper method to wait between actions like a real user
    /// </summary>
    protected async Task WaitForUserPaceAsync()
    {
        if (Configuration.SmkSimulation.SimulateRealUserSpeed)
        {
            await Task.Delay(Configuration.SmkSimulation.DelayBetweenActions);
        }
    }

    private TestConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables();

        var config = builder.Build();
        var testConfig = new TestConfiguration();
        config.GetSection("E2ETests").Bind(testConfig);
        
        return testConfig;
    }

    private ILogger ConfigureLogging()
    {
        var logPath = Path.Combine("Reports", "Logs", $"e2e_test_{DateTime.Now:yyyyMMdd}.log");
        
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    private void CreateTestDirectories()
    {
        Directory.CreateDirectory(Configuration.VideoPath);
        Directory.CreateDirectory(Configuration.TracePath);
        Directory.CreateDirectory(Configuration.ScreenshotPath);
        Directory.CreateDirectory(Path.Combine("Reports", "Logs"));
    }
}