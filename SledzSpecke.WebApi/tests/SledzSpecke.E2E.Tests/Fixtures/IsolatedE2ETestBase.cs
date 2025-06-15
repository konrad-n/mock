using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Serilog;
using SledzSpecke.E2E.Tests.Core;
using SledzSpecke.E2E.Tests.Infrastructure;
using Xunit;

namespace SledzSpecke.E2E.Tests.Fixtures;

/// <summary>
/// Base class for E2E tests with database isolation
/// Each test gets a fresh database that's cleaned up after test completion
/// </summary>
public abstract class IsolatedE2ETestBase : IAsyncLifetime
{
    private TestDatabaseManager _databaseManager = null!;
    private string _testConnectionString = null!;
    private TestDatabaseSnapshot? _initialSnapshot;
    
    protected IBrowserFactory BrowserFactory { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected TestConfiguration Configuration { get; private set; } = null!;
    protected ILogger Logger { get; private set; } = null!;
    
    /// <summary>
    /// Override to specify whether to use snapshot restoration between tests
    /// </summary>
    protected virtual bool UseSnapshotRestore => true;
    
    /// <summary>
    /// Override to specify tables to clean between tests (if not using snapshots)
    /// </summary>
    protected virtual string[] TablesToClean => new[] 
    { 
        "MedicalShifts", 
        "Procedures", 
        "Users", 
        "RefreshTokens" 
    };

    public async Task InitializeAsync()
    {
        // Setup configuration and logging
        Configuration = LoadConfiguration();
        Logger = ConfigureLogging();
        
        Logger.Information("Initializing isolated E2E test: {TestName}", GetType().Name);
        
        // Create IConfiguration from TestConfiguration
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = Configuration.TestDatabaseConnectionString ?? "Host=localhost;Database=sledzspecke_db;Username=postgres;Password=postgres"
        });
        var config = configBuilder.Build();
        
        // Initialize test database
        _databaseManager = new TestDatabaseManager(config, Logger);
        _testConnectionString = await _databaseManager.InitializeAsync();
        
        // Update configuration with test connection string
        UpdateConfigurationWithTestDatabase(_testConnectionString);
        
        // Create initial snapshot if using snapshot restore
        if (UseSnapshotRestore)
        {
            _initialSnapshot = await _databaseManager.CreateSnapshotAsync("initial");
        }
        
        // Create browser factory and page
        BrowserFactory = new BrowserFactory(Configuration, Logger);
        Context = await BrowserFactory.CreateBrowserContextAsync();
        Page = await Context.NewPageAsync();
        
        // Enable request/response logging for debugging
        Page.Request += (_, request) => Logger.Debug("Request: {Method} {Url}", request.Method, request.Url);
        Page.Response += (_, response) => 
        {
            if (response.Status >= 400)
            {
                Logger.Warning("Response error: {Status} {Url}", response.Status, response.Url);
            }
        };
        
        // Create necessary directories
        CreateTestDirectories();
        
        // Custom initialization
        await OnInitializeAsync();
    }

    public async Task DisposeAsync()
    {
        Logger.Information("Disposing isolated E2E test: {TestName}", GetType().Name);
        
        try
        {
            // Custom cleanup
            await OnDisposeAsync();
            
            // Take screenshot if test failed
            if (TestContext.Current?.Outcome == TestOutcome.Failed)
            {
                await TakeFailureScreenshotAsync(GetType().Name);
            }
            
            // Close browser
            if (Context != null)
            {
                await Context.CloseAsync();
            }
            
            if (BrowserFactory != null)
            {
                await BrowserFactory.DisposeAsync();
            }
            
            // Cleanup database
            if (_databaseManager != null)
            {
                await _databaseManager.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error during test disposal");
        }
    }

    /// <summary>
    /// Resets database to initial state - useful for multi-scenario tests
    /// </summary>
    protected async Task ResetDatabaseAsync()
    {
        if (UseSnapshotRestore && _initialSnapshot != null)
        {
            Logger.Information("Restoring database from initial snapshot");
            await _databaseManager.RestoreSnapshotAsync(_initialSnapshot);
        }
        else
        {
            Logger.Information("Cleaning database tables");
            await _databaseManager.CleanTablesAsync(TablesToClean);
        }
    }

    /// <summary>
    /// Creates a database snapshot at current state
    /// </summary>
    protected async Task<TestDatabaseSnapshot> CreateSnapshotAsync(string name)
    {
        return await _databaseManager.CreateSnapshotAsync(name);
    }

    /// <summary>
    /// Restores database from a specific snapshot
    /// </summary>
    protected async Task RestoreSnapshotAsync(TestDatabaseSnapshot snapshot)
    {
        await _databaseManager.RestoreSnapshotAsync(snapshot);
    }

    /// <summary>
    /// Executes raw SQL for test setup
    /// </summary>
    protected async Task ExecuteSqlAsync(string sql)
    {
        await _databaseManager.ExecuteSqlAsync(sql);
    }

    /// <summary>
    /// Override to perform custom initialization
    /// </summary>
    protected virtual Task OnInitializeAsync() => Task.CompletedTask;

    /// <summary>
    /// Override to perform custom cleanup
    /// </summary>
    protected virtual Task OnDisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// Helper to take failure screenshot
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
    /// Helper to simulate user typing
    /// </summary>
    protected async Task TypeLikeUserAsync(string selector, string text)
    {
        await Page.ClickAsync(selector);
        
        if (Configuration.SmkSimulation.SimulateRealUserSpeed)
        {
            // Simulate real typing with delay
            for (int i = 0; i < text.Length; i++)
            {
                await Page.Keyboard.TypeAsync(text[i].ToString());
                await Task.Delay(50);
            }
        }
        else
        {
            await Page.FillAsync(selector, text);
        }
    }

    /// <summary>
    /// Helper to wait between actions
    /// </summary>
    protected async Task WaitForUserPaceAsync()
    {
        if (Configuration.SmkSimulation.SimulateRealUserSpeed)
        {
            await Task.Delay(Configuration.SmkSimulation.DelayBetweenActions);
        }
    }

    /// <summary>
    /// Helper to wait for API response
    /// </summary>
    protected async Task<IResponse> WaitForApiResponseAsync(string urlPattern, Func<Task> action)
    {
        var responseTask = Page.WaitForResponseAsync(response => 
            response.Url.Contains(urlPattern) && response.Status == 200);
        
        await action();
        
        return await responseTask;
    }

    private TestConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.E2E.json", optional: true)
            .AddEnvironmentVariables();

        var config = builder.Build();
        var testConfig = new TestConfiguration();
        config.GetSection("E2ETests").Bind(testConfig);
        
        return testConfig;
    }

    private void UpdateConfigurationWithTestDatabase(string connectionString)
    {
        // Update the base URL to use test database
        // This assumes the API can accept connection string override via header or query param
        // Alternatively, we'd need to start a test instance of the API
        Configuration.TestDatabaseConnectionString = connectionString;
    }

    private ILogger ConfigureLogging()
    {
        var logPath = Path.Combine("Reports", "Logs", $"e2e_isolated_{DateTime.Now:yyyyMMdd}.log");
        
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

/// <summary>
/// Test context helper for tracking test outcomes
/// </summary>
public class TestContext
{
    public static TestContext? Current { get; set; }
    public TestOutcome Outcome { get; set; }
}

public enum TestOutcome
{
    Passed,
    Failed,
    Skipped
}