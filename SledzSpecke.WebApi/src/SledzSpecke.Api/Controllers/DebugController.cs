using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IPasswordManager _passwordManager;
    private readonly IUserRepository _userRepository;
    
    public DebugController(IPasswordManager passwordManager, IUserRepository userRepository)
    {
        _passwordManager = passwordManager;
        _userRepository = userRepository;
    }
    
    [HttpGet("test-password")]
    public async Task<IActionResult> TestPassword([FromQuery] string email, [FromQuery] string password)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            
            if (user == null)
            {
                return Ok(new { found = false, message = "User not found" });
            }
            
            var isValid = _passwordManager.Verify(password, user.Password);
            
            return Ok(new 
            { 
                found = true,
                email = user.Email,
                storedHash = user.Password,
                passwordProvided = password,
                isValid = isValid,
                userId = user.UserId
            });
        }
        catch (Exception ex)
        {
            return Ok(new { error = ex.Message });
        }
    }
    
    [HttpGet("generate-hash")]
    public IActionResult GenerateHash([FromQuery] string password)
    {
        var hash = _passwordManager.Secure(password);
        return Ok(new { password, hash });
    }
}