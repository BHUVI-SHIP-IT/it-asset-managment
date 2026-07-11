namespace Tracer.Shared.Authorization;

/// <summary>
/// Shared permission satisfaction logic for backend authorization and frontend mirrors.
/// A resource-level Manage permission grants all actions on that resource.
/// </summary>
public static class PermissionChecker
{
    public static bool Satisfies(IEnumerable<string> userPermissions, string requiredPermission)
    {
        var perms = userPermissions as ICollection<string> ?? userPermissions.ToList();

        if (perms.Contains(requiredPermission))
            return true;

        var manage = Permissions.GetManagePermission(requiredPermission);
        return manage is not null && perms.Contains(manage);
    }
}
