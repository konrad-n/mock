using Microsoft.Playwright;
using Serilog;

namespace SledzSpecke.E2E.Tests.Core;

/// <summary>
/// Factory for creating browser instances following Factory pattern and Dependency Inversion Principle
/// </summary>
public interface IBrowserFactory : IAsyncDisposable
{
    Task<IBrowserContext> CreateBrowserContextAsync();
    Task<IPage> CreatePageAsync();
}

public class BrowserFactory : IBrowserFactory
{
    private readonly TestConfiguration _configuration;
    private readonly ILogger _logger;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public BrowserFactory(TestConfiguration configuration, ILogger logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IBrowserContext> CreateBrowserContextAsync()
    {
        await EnsureBrowserInitializedAsync();
        
        var contextOptions = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            IgnoreHTTPSErrors = true,
            Locale = "pl-PL",
            TimezoneId = "Europe/Warsaw"
        };

        if (_configuration.RecordVideo)
        {
            contextOptions.RecordVideoDir = _configuration.VideoPath;
            contextOptions.RecordVideoSize = new RecordVideoSize { Width = 1920, Height = 1080 };
        }

        var context = await _browser!.NewContextAsync(contextOptions);

        if (_configuration.TraceEnabled)
        {
            await context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        _logger.Information("Browser context created with options: {@Options}", contextOptions);
        
        return context;
    }

    public async Task<IPage> CreatePageAsync()
    {
        var context = await CreateBrowserContextAsync();
        var page = await context.NewPageAsync();
        
        // Set default timeout
        page.SetDefaultTimeout(_configuration.DefaultTimeout);
        
        // Add request/response logging
        page.Request += (_, request) => 
            _logger.Debug("Request: {Method} {Url}", request.Method, request.Url);
            
        page.Response += (_, response) =>
        {
            if (!response.Ok && response.Status >= 400)
            {
                _logger.Warning("Response: {Status} {Url}", response.Status, response.Url);
            }
        };

        page.PageError += (_, error) =>
            _logger.Error("Page error: {Error}", error);

        _logger.Information("Page created with default timeout: {Timeout}ms", _configuration.DefaultTimeout);
        
        return page;
    }

    private async Task EnsureBrowserInitializedAsync()
    {
        if (_playwright == null)
        {
            _playwright = await Playwright.CreateAsync();
            _logger.Information("Playwright initialized");
        }

        if (_browser == null)
        {
            _browser = await LaunchBrowserAsync();
            _logger.Information("Browser launched: {BrowserType}", _configuration.Browser);
        }
    }

    private async Task<IBrowser> LaunchBrowserAsync()
    {
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _configuration.Headless,
            SlowMo = _configuration.SmkSimulation.SimulateRealUserSpeed ? 100 : 0,
            Args = new[] { "--start-maximized" }
        };

        return _configuration.Browser switch
        {
            BrowserType.Chromium => await _playwright!.Chromium.LaunchAsync(launchOptions),
            BrowserType.Firefox => await _playwright!.Firefox.LaunchAsync(launchOptions),
            BrowserType.Safari => await _playwright!.Webkit.LaunchAsync(launchOptions),
            _ => throw new NotSupportedException($"Browser type {_configuration.Browser} is not supported")
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _logger.Information("Browser closed");
        }

        _playwright?.Dispose();
        _logger.Information("Playwright disposed");
    }
}