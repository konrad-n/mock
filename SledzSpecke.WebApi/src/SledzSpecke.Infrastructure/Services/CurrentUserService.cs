using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SledzSpecke.Core.Services;

namespace SledzSpecke.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirst("id");
            return userIdClaim?.Value;
        }
    }

    public string? Email
    {
        get
        {
            var emailClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)
                          ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email");
            return emailClaim?.Value;
        }
    }

    public string? Name
    {
        get
        {
            var nameClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)
                         ?? _httpContextAccessor.HttpContext?.User?.FindFirst("name");
            return nameClaim?.Value;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}