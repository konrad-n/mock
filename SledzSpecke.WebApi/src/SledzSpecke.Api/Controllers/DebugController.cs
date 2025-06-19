using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Security;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly IPasswordManager _passwordManager;
    private readonly ILogger<DebugController> _logger;

    public DebugController(IPasswordManager passwordManager, ILogger<DebugController> logger)
    {
        _passwordManager = passwordManager;
        _logger = logger;
    }

    [HttpPost("hash-password")]
    public IActionResult HashPassword([FromBody] string password)
    {
        if (string.IsNullOrEmpty(password))
            return BadRequest("Password is required");

        var hashedPassword = _passwordManager.Secure(password);
        _logger.LogInformation("Generated hash for password: {Hash}", hashedPassword);
        
        return Ok(new { 
            password = password,
            hash = hashedPassword,
            hashLength = hashedPassword.Length
        });
    }

    [HttpPost("verify-password")]
    public IActionResult VerifyPassword([FromBody] VerifyPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Hash))
            return BadRequest("Password and hash are required");

        var isValid = _passwordManager.Verify(request.Password, request.Hash);
        _logger.LogInformation("Password verification result: {IsValid}", isValid);
        
        return Ok(new { 
            isValid = isValid,
            password = request.Password,
            hashLength = request.Hash.Length
        });
    }
}

public class VerifyPasswordRequest
{
    public string Password { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
}