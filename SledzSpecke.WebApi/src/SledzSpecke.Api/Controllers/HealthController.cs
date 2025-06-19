using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SledzSpecke.Infrastructure.DAL;
using System.Reflection;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly SledzSpeckeDbContext _dbContext;
    private readonly ILogger<HealthController> _logger;

    public HealthController(SledzSpeckeDbContext dbContext, ILogger<HealthController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    [HttpHead]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Check database connectivity
            var canConnect = await _dbContext.Database.CanConnectAsync();
            
            var health = new
            {
                status = canConnect ? "healthy" : "unhealthy",
                timestamp = DateTime.UtcNow,
                services = new
                {
                    database = canConnect ? "connected" : "disconnected",
                    api = "running"
                },
                version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                uptime = GetUptime()
            };

            if (!canConnect)
            {
                _logger.LogWarning("Health check failed - database connection issue");
                return StatusCode(503, health);
            }

            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed with exception");
            return StatusCode(503, new
            {
                status = "unhealthy",
                timestamp = DateTime.UtcNow,
                error = "Health check failed",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        }
    }

    [HttpOptions]
    public IActionResult Options()
    {
        // Return 204 No Content for OPTIONS requests (CORS preflight)
        return NoContent();
    }
    
    private string GetUptime()
    {
        var uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
        return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
    }
}