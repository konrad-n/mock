using SledzSpecke.Application;
using SledzSpecke.Core;
using SledzSpecke.Infrastructure;
using SledzSpecke.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCore()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices();

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo
        .Console()
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