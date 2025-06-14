using SledzSpecke.Application;
using SledzSpecke.Core;
using SledzSpecke.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCore()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo
        .Console()
        .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Use centralized infrastructure setup
app.UseInfrastructure();

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