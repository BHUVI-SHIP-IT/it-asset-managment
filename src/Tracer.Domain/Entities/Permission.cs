using Tracer.Domain.Common;

namespace Tracer.Domain.Entities;

/// <summary>
/// RBAC Permission (Doc 7 §3). Follows Resource.Action naming (e.g. Assets.Create).
/// </summary>
public class Permission : Entity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public Permission(int id) : base(id) { }
    private Permission() { }
}
