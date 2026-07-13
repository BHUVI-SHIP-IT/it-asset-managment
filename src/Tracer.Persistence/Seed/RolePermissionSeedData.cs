using Microsoft.EntityFrameworkCore;
using Tracer.Domain.Entities;
using Tracer.Persistence.Contexts;
using Tracer.Shared.Authorization;
using AuthRoles = Tracer.Shared.Authorization.Roles;

namespace Tracer.Persistence.Seed;

public static class RolePermissionSeedData
{
    /// <summary>Self-service permissions granted to the Employee role.</summary>
    public static readonly string[] EmployeeSelfServicePermissions =
    [
        Permissions.Requests.Create,
        Permissions.Requests.ViewOwn,
        Permissions.Notifications.View
    ];

    public static void SeedRolesAndPermissions(ModelBuilder builder)
    {
        var roles = new[]
        {
            new { Id = 1, Name = AuthRoles.SuperAdmin, Description = "Ultimate authority over the Tracer system" },
            new { Id = 2, Name = AuthRoles.SystemAdmin, Description = "Technical management of ITAM application" },
            new { Id = 3, Name = AuthRoles.ITAdmin, Description = "Oversight of IT assets and workflows" },
            new { Id = 4, Name = AuthRoles.AssetManager, Description = "Strategic management of asset portfolio" },
            new { Id = 5, Name = AuthRoles.InventoryManager, Description = "Daily physical management of inventory" },
            new { Id = 6, Name = AuthRoles.ProcurementOfficer, Description = "Managing supplier and inbound purchases" },
            new { Id = 7, Name = AuthRoles.DepartmentManager, Description = "Oversight of departmental assets" },
            new { Id = 8, Name = AuthRoles.FinanceOfficer, Description = "Managing financial lifecycle of assets" },
            new { Id = 9, Name = AuthRoles.Auditor, Description = "Independent verification of compliance" },
            new { Id = 10, Name = AuthRoles.HelpDesk, Description = "Frontline support managing deployments" },
            new { Id = 11, Name = AuthRoles.Employee, Description = "Standard end-user" },
            new { Id = 12, Name = AuthRoles.Guest, Description = "Unauthenticated or restricted access" }
        };

        foreach (var role in roles)
        {
            builder.Entity<Role>().HasData(new Role(role.Id) { Name = role.Name, Description = role.Description });
        }

        var permissions = new List<Permission>();
        for (var i = 0; i < Permissions.All.Length; i++)
        {
            var name = Permissions.All[i];
            var p = new Permission(i + 1) { Name = name, Description = $"Allows {name}" };
            permissions.Add(p);
            builder.Entity<Permission>().HasData(p);
        }

        var permDict = permissions.ToDictionary(p => p.Name, p => p.Id);
        var rolePermissions = new List<object>();

        foreach (var p in permissions)
        {
            rolePermissions.Add(new { RoleId = 1, PermissionId = p.Id });
        }

        var helpDeskPerms = new[]
        {
            Permissions.Assets.View, Permissions.Assets.CheckOut, Permissions.Assets.CheckIn,
            Permissions.Maintenance.Manage, Permissions.Users.View
        };
        foreach (var name in helpDeskPerms)
        {
            rolePermissions.Add(new { RoleId = 10, PermissionId = permDict[name] });
        }

        var invMgrPerms = new[]
        {
            Permissions.Assets.View, Permissions.Assets.Create, Permissions.Assets.Edit,
            Permissions.Assets.CheckOut, Permissions.Assets.CheckIn,
            Permissions.Consumables.Manage, Permissions.Components.Manage,
            Permissions.Accessories.Manage, Permissions.Licenses.Manage
        };
        foreach (var name in invMgrPerms)
        {
            rolePermissions.Add(new { RoleId = 5, PermissionId = permDict[name] });
        }

        // Employee (RoleId 11): self-service only — no Assets.View so org-wide dashboard stays 403.
        foreach (var name in EmployeeSelfServicePermissions)
        {
            rolePermissions.Add(new { RoleId = 11, PermissionId = permDict[name] });
        }

        builder.Entity<RolePermission>().HasData(rolePermissions);

        var defaultCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        builder.Entity<Company>().HasData(new Company(defaultCompanyId)
        {
            Name = "Tracer Headquarters"
        });

        var adminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var passwordHash = "$2a$11$AeZacdZCy9zUfbQyUnd3fuVX.rG9K1Cslyg8YK5Z6tBIPiTG5V6pe"; // Admin123!

        builder.Entity<ApplicationUser>().HasData(new ApplicationUser(adminUserId)
        {
            FullName = "System Administrator",
            Email = "admin@tracer.io",
            PasswordHash = passwordHash,
            CompanyId = defaultCompanyId,
            RoleId = 1,
            IsActive = true
        });
    }

    /// <summary>
    /// Idempotent grant of Employee self-service permissions (covers DBs created before the seed migration).
    /// </summary>
    public static async Task EnsureEmployeePermissionsAsync(
        TracerDbContext db,
        CancellationToken cancellationToken = default)
    {
        const int employeeRoleId = 11;

        var needed = await db.Permissions
            .AsNoTracking()
            .Where(p => EmployeeSelfServicePermissions.Contains(p.Name))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (needed.Count == 0)
            return;

        var existing = await db.RolePermissions
            .Where(rp => rp.RoleId == employeeRoleId && needed.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .ToListAsync(cancellationToken);

        foreach (var permissionId in needed.Except(existing))
        {
            db.RolePermissions.Add(new RolePermission
            {
                RoleId = employeeRoleId,
                PermissionId = permissionId
            });
        }

        if (db.ChangeTracker.HasChanges())
            await db.SaveChangesAsync(cancellationToken);
    }
}
