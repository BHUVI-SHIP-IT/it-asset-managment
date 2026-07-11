using Microsoft.AspNetCore.Authorization;
using Tracer.Shared.Authorization;
using AuthRoles = Tracer.Shared.Authorization.Roles;

namespace Tracer.Infrastructure.Authentication;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity is null || !context.User.Identity.IsAuthenticated)
        {
            return Task.CompletedTask;
        }

        if (context.User.IsInRole(AuthRoles.SuperAdmin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var permissions = context.User.FindAll("permissions").Select(c => c.Value);

        if (PermissionChecker.Satisfies(permissions, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
