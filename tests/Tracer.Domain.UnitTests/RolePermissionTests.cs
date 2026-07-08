using Tracer.Domain.Entities;
using Xunit;

namespace Tracer.Domain.UnitTests;

public class RolePermissionTests
{
    [Fact]
    public void Role_Should_InitializeWithEmptyPermissions()
    {
        var role = new Role(1) { Name = "TestRole" };
        
        Assert.NotNull(role.RolePermissions);
        Assert.Empty(role.RolePermissions);
    }
    
    [Fact]
    public void Permission_Should_InitializeWithEmptyRoles()
    {
        var permission = new Permission(1) { Name = "TestPermission" };
        
        Assert.NotNull(permission.RolePermissions);
        Assert.Empty(permission.RolePermissions);
    }
}
