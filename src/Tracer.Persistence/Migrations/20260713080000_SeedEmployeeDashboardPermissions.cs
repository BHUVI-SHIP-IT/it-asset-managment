using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Tracer.Persistence.Contexts;

#nullable disable

namespace Tracer.Persistence.Migrations;

/// <inheritdoc />
[DbContext(typeof(TracerDbContext))]
[Migration("20260713080000_SeedEmployeeDashboardPermissions")]
public partial class SeedEmployeeDashboardPermissions : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Employee (RoleId 11): self-service permissions for user dashboard / requests.
        // 23 = Notifications.View, 81 = Requests.Create, 82 = Requests.ViewOwn
        // Idempotent: safe if EnsureEmployeePermissionsAsync already inserted rows.
        migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [RoleId] = 11 AND [PermissionId] = 23)
                INSERT INTO [RolePermissions] ([RoleId], [PermissionId]) VALUES (11, 23);
            IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [RoleId] = 11 AND [PermissionId] = 81)
                INSERT INTO [RolePermissions] ([RoleId], [PermissionId]) VALUES (11, 81);
            IF NOT EXISTS (SELECT 1 FROM [RolePermissions] WHERE [RoleId] = 11 AND [PermissionId] = 82)
                INSERT INTO [RolePermissions] ([RoleId], [PermissionId]) VALUES (11, 82);
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE FROM [RolePermissions] WHERE [RoleId] = 11 AND [PermissionId] IN (23, 81, 82);
            """);
    }
}
