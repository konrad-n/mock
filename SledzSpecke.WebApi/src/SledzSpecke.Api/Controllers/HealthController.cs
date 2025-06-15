using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SledzSpecke.Infrastructure.DAL;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<IActionResult> Get()
    {
        try
        {
            // Check database connectivity
            var canConnect = await _dbContext.Database.CanConnectAsync();
            
            var health = new
            {
                status = canConnect ? "Healthy" : "Unhealthy",
                timestamp = DateTime.UtcNow,
                services = new
                {
                    database = canConnect ? "Connected" : "Disconnected",
                    api = "Running"
                },
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
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
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }
}