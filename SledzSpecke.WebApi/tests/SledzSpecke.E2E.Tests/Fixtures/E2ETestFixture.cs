using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using SledzSpecke.E2E.Tests.Infrastructure;

namespace SledzSpecke.E2E.Tests.Fixtures;

/// <summary>
/// Main test fixture that provides shared resources for E2E tests
/// </summary>
public class E2ETestFixture : IDisposable
{
    public IConfiguration Configuration { get; }
    public IServiceProvider Services { get; }
    public TestDatabaseManager DatabaseManager { get; }

    public E2ETestFixture()
    {
        // Build configuration
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .WriteTo.Console()
            .WriteTo.File("logs/e2e-tests-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Build service provider
        var services = new ServiceCollection();
        services.AddSingleton(Configuration);
        services.AddLogging(builder => builder.AddSerilog());
        services.AddSingleton<TestDatabaseManager>();

        Services = services.BuildServiceProvider();
        
        // Get database manager
        DatabaseManager = Services.GetRequiredService<TestDatabaseManager>();
        
        Log.Information("E2E Test Fixture initialized with environment: {Environment}", environmentName);
        Log.Information("API URL: {ApiUrl}", Configuration["E2ETests:ApiUrl"]);
        Log.Information("Base URL: {BaseUrl}", Configuration["E2ETests:BaseUrl"]);
    }

    public void Dispose()
    {
        Log.Information("E2E Test Fixture disposing...");
        Log.CloseAndFlush();
    }
}