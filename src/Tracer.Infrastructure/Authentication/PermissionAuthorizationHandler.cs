using Microsoft.AspNetCore.Authorization;

namespace Tracer.Infrastructure.Authentication;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity is null || !context.User.Identity.IsAuthenticated)
        {
            return Task.CompletedTask;
        }

        // Check if user has the specific permission
        var permissions = context.User.FindAll("permissions").Select(c => c.Value);
        
        // SuperAdmin gets a free pass or we check exact matches
        if (permissions.Contains(requirement.Permission) || context.User.HasClaim("role", "SuperAdmin"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
