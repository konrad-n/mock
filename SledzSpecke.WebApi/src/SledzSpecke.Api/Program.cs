using SledzSpecke.Application;
using SledzSpecke.Core;
using SledzSpecke.Infrastructure;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Infrastructure.DAL.Seeding;
using SledzSpecke.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCore()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the exception handling middleware
builder.Services.AddSingleton<ExceptionHandlingMiddleware>();

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo
        .Console()
        .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add exception handling middleware early in the pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database and seed data
try
{
    await InitializeDatabaseAsync(app);
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Database initialization failed, but continuing with application startup.");
}

app.Run();

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Starting database initialization...");
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Check if we need to run migrations
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation($"Applying {pendingMigrations.Count()} pending migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully.");
        }
        
        // Seed specialization templates if no data exists
        var hasSpecializations = await context.Specializations.AnyAsync();
        if (!hasSpecializations)
        {
            logger.LogInformation("Seeding specialization templates...");
            await seeder.SeedSpecializationTemplatesAsync();
        }
        
        // Seed basic data
        var hasUsers = await context.Users.AnyAsync();
        if (!hasUsers)
        {
            await seeder.SeedBasicDataAsync();
        }
        
        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization.");
        throw;
    }
}