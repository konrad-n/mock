using Microsoft.AspNetCore.Authorization;

namespace SledzSpecke.Api.Authorization;

public class AdminOnlyRequirement : IAuthorizationRequirement
{
}

public class AdminOnlyAuthorizationHandler : AuthorizationHandler<AdminOnlyRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        AdminOnlyRequirement requirement)
    {
        // For now, check if user has admin claim
        // In production, this should check against a proper admin role/claim
        if (context.User.HasClaim(c => c.Type == "role" && c.Value == "admin") ||
            context.User.HasClaim(c => c.Type == "admin" && c.Value == "true"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}