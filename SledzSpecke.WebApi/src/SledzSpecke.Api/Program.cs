using SledzSpecke.Application;
using SledzSpecke.Core;
using SledzSpecke.Infrastructure;
using SledzSpecke.Api.Extensions;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using Microsoft.EntityFrameworkCore;
using SledzSpecke.Infrastructure.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Handle migrate-database command
if (args.Length > 0 && args[0] == "migrate-database")
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    var services = new ServiceCollection();
    services.AddDbContext<SledzSpeckeDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    
    var serviceProvider = services.BuildServiceProvider();
    using var scope = serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
    
    try
    {
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Database migration completed successfully.");
        Environment.Exit(0);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message}");
        Environment.Exit(1);
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCore()
    .AddApplicationFixed()  // Use the fixed version without circular dependencies
    .AddInfrastructure(builder.Configuration)
    .AddApiServices();

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithExceptionDetails()
        .Enrich.WithProperty("Application", "SledzSpecke")
        .Enrich.WithProperty("Version", typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0")
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File(
            path: "/var/log/sledzspecke/api-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "/var/log/sledzspecke/structured-.json",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            formatter: new Serilog.Formatting.Json.JsonFormatter())
        .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Use enhanced API middleware setup
app.UseApiMiddleware();

// Initialize database and seed data
try
{
    await app.InitializeDatabaseAsync();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Database initialization failed, but continuing with application startup.");
}

app.Run();

public partial class Program { }