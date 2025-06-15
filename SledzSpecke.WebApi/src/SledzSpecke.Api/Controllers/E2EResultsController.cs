using Microsoft.AspNetCore.Mvc;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/e2e")]
public class E2EResultsController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<E2EResultsController> _logger;

    public E2EResultsController(IWebHostEnvironment environment, ILogger<E2EResultsController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpGet("results")]
    [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger in production
    public IActionResult GetResults()
    {
        // This endpoint was used for demo purposes only
        // In production, E2E test results should be accessed through CI/CD pipeline
        if (_environment.IsProduction())
        {
            return NotFound();
        }
        
        return Ok(new 
        { 
            message = "E2E results endpoint is disabled in production. Use CI/CD pipeline for test results.",
            documentation = "https://github.com/konrad-n/mock/actions"
        });
    }
}