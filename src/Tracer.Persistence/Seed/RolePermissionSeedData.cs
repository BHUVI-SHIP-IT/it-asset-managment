using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracer.Domain.Entities;

namespace Tracer.Persistence.Seed;

public static class RolePermissionSeedData
{
    public static void SeedRolesAndPermissions(ModelBuilder builder)
    {
        // 1. Seed Roles (Doc 7 §2)
        var roles = new[]
        {
            new { Id = 1, Name = "SuperAdmin", Description = "Ultimate authority over the Tracer system" },
            new { Id = 2, Name = "SystemAdmin", Description = "Technical management of ITAM application" },
            new { Id = 3, Name = "ITAdmin", Description = "Oversight of IT assets and workflows" },
            new { Id = 4, Name = "AssetManager", Description = "Strategic management of asset portfolio" },
            new { Id = 5, Name = "InventoryManager", Description = "Daily physical management of inventory" },
            new { Id = 6, Name = "ProcurementOfficer", Description = "Managing supplier and inbound purchases" },
            new { Id = 7, Name = "DepartmentManager", Description = "Oversight of departmental assets" },
            new { Id = 8, Name = "FinanceOfficer", Description = "Managing financial lifecycle of assets" },
            new { Id = 9, Name = "Auditor", Description = "Independent verification of compliance" },
            new { Id = 10, Name = "HelpDesk", Description = "Frontline support managing deployments" },
            new { Id = 11, Name = "Employee", Description = "Standard end-user" },
            new { Id = 12, Name = "Guest", Description = "Unauthenticated or restricted access" }
        };

        foreach (var role in roles)
        {
            builder.Entity<Role>().HasData(new Role(role.Id) { Name = role.Name, Description = role.Description });
        }

        // 2. Seed Permissions (Doc 7 §3)
        var permNames = new[]
        {
            "Assets.View", "Assets.Create", "Assets.Edit", "Assets.Delete",
            "Assets.Assign", "Assets.CheckOut", "Assets.CheckIn", "Assets.Transfer",
            "Assets.Clone", "Assets.Dispose", "Assets.Archive",

            "Users.View", "Users.Create", "Users.Edit", "Users.Delete",

            "Roles.Manage", "Permissions.Manage",

            "Reports.View", "Reports.Export",

            "Settings.Manage", "API.Manage", "Notifications.Manage",

            "Maintenance.Manage", "AuditLogs.View",

            "Licenses.Manage", "Accessories.Manage", "Components.Manage", "Consumables.Manage"
        };

        var permissions = new List<Permission>();
        for (int i = 0; i < permNames.Length; i++)
        {
            var p = new Permission(i + 1) { Name = permNames[i], Description = $"Allows {permNames[i]}" };
            permissions.Add(p);
            builder.Entity<Permission>().HasData(p);
        }

        // 3. Seed RolePermissions (Doc 7 §4.2)
        // Helper to get permission IDs
        var permDict = permissions.ToDictionary(p => p.Name, p => p.Id);

        var rolePermissions = new List<object>();

        // Super Admin gets everything
        foreach (var p in permissions)
        {
            rolePermissions.Add(new { RoleId = 1, PermissionId = p.Id });
        }

        // Help Desk (Role 10)
        var helpDeskPerms = new[] { "Assets.View", "Assets.CheckOut", "Assets.CheckIn", "Maintenance.Manage", "Users.View" };
        foreach (var name in helpDeskPerms)
        {
            rolePermissions.Add(new { RoleId = 10, PermissionId = permDict[name] });
        }

        // Inventory Manager (Role 5)
        var invMgrPerms = new[] { "Assets.View", "Assets.Create", "Assets.Edit", "Assets.CheckOut", "Assets.CheckIn" };
        foreach (var name in invMgrPerms)
        {
            rolePermissions.Add(new { RoleId = 5, PermissionId = permDict[name] });
        }

        builder.Entity<RolePermission>().HasData(rolePermissions);

        // 4. Seed default Company and SuperAdmin User (For M1 Testing)
        var defaultCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        builder.Entity<Company>().HasData(new Company(defaultCompanyId)
        {
            Name = "Tracer Headquarters"
        });

        var adminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        // Password is 'Admin123!' hashed with BCrypt.
        var passwordHash = "$2a$11$N9V2V2W41q4.F854hV5/Z.tJjU.n/q.4mO1h3Z/g71.p3z7g91/m6";

        var adminUser = new ApplicationUser(adminUserId)
        {
            FullName = "System Administrator",
            Email = "admin@tracer.io",
            PasswordHash = passwordHash,
            CompanyId = defaultCompanyId,
            RoleId = 1, // SuperAdmin
            IsActive = true
        };

        builder.Entity<ApplicationUser>().HasData(adminUser);
    }
}
