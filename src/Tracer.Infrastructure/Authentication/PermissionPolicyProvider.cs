using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Tracer.Infrastructure.Authentication;

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // First try to find a standard built-in policy
        var policy = await base.GetPolicyAsync(policyName);
        if (policy is null)
        {
            // If it's not a standard policy, assume the policyName is the permission name (e.g. "Assets.Create")
            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
        }

        return policy;
    }
}
