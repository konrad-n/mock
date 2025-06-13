using Microsoft.AspNetCore.Http;
using SledzSpecke.Application.Abstractions;
using System.Security.Claims;

namespace SledzSpecke.Infrastructure.Auth;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException("Invalid user ID format");
        }

        return userId;
    }
}