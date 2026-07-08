using Tracer.Domain.Common;

namespace Tracer.Domain.Entities;

/// <summary>
/// RBAC Role (Doc 7 §2). Maps to a set of permissions.
/// </summary>
public class Role : Entity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public Role(int id) : base(id) { }
    private Role() { }
}
