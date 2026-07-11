using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tracer.Persistence.Migrations;

/// <inheritdoc />
public partial class UpdatePermissionSeedData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 23, 10 });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 20,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Settings.View", "Settings.View" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 21,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Settings.Manage", "Settings.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 22,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows API.Manage", "API.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 23,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Notifications.View", "Notifications.View" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 24,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Notifications.Delete", "Notifications.Delete" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 25,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Notifications.Manage", "Notifications.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 26,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Maintenance.Manage", "Maintenance.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 27,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows AuditLogs.View", "AuditLogs.View" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 28,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Categories.View", "Categories.View" });

        migrationBuilder.InsertData(
            table: "Permissions",
            columns: new[] { "Id", "Description", "Name" },
            values: new object[,]
            {
                { 29, "Allows Categories.Create", "Categories.Create" },
                { 30, "Allows Categories.Update", "Categories.Update" },
                { 31, "Allows Categories.Delete", "Categories.Delete" },
                { 32, "Allows Locations.View", "Locations.View" },
                { 33, "Allows Locations.Create", "Locations.Create" },
                { 34, "Allows Locations.Update", "Locations.Update" },
                { 35, "Allows Locations.Delete", "Locations.Delete" },
                { 36, "Allows Departments.View", "Departments.View" },
                { 37, "Allows Departments.Create", "Departments.Create" },
                { 38, "Allows Departments.Update", "Departments.Update" },
                { 39, "Allows Departments.Delete", "Departments.Delete" },
                { 40, "Allows Manufacturers.View", "Manufacturers.View" },
                { 41, "Allows Manufacturers.Create", "Manufacturers.Create" },
                { 42, "Allows Manufacturers.Update", "Manufacturers.Update" },
                { 43, "Allows Manufacturers.Delete", "Manufacturers.Delete" },
                { 44, "Allows Suppliers.View", "Suppliers.View" },
                { 45, "Allows Suppliers.Create", "Suppliers.Create" },
                { 46, "Allows Suppliers.Update", "Suppliers.Update" },
                { 47, "Allows Suppliers.Delete", "Suppliers.Delete" },
                { 48, "Allows StatusLabels.View", "StatusLabels.View" },
                { 49, "Allows StatusLabels.Create", "StatusLabels.Create" },
                { 50, "Allows StatusLabels.Update", "StatusLabels.Update" },
                { 51, "Allows StatusLabels.Delete", "StatusLabels.Delete" },
                { 52, "Allows AssetModels.View", "AssetModels.View" },
                { 53, "Allows AssetModels.Create", "AssetModels.Create" },
                { 54, "Allows AssetModels.Update", "AssetModels.Update" },
                { 55, "Allows AssetModels.Delete", "AssetModels.Delete" },
                { 56, "Allows CustomFields.View", "CustomFields.View" },
                { 57, "Allows CustomFields.Manage", "CustomFields.Manage" },
                { 58, "Allows Consumables.View", "Consumables.View" },
                { 59, "Allows Consumables.Create", "Consumables.Create" },
                { 60, "Allows Consumables.Update", "Consumables.Update" },
                { 61, "Allows Consumables.Delete", "Consumables.Delete" },
                { 62, "Allows Consumables.Checkout", "Consumables.Checkout" },
                { 63, "Allows Consumables.Manage", "Consumables.Manage" },
                { 64, "Allows Licenses.View", "Licenses.View" },
                { 65, "Allows Licenses.Create", "Licenses.Create" },
                { 66, "Allows Licenses.Update", "Licenses.Update" },
                { 67, "Allows Licenses.Delete", "Licenses.Delete" },
                { 68, "Allows Licenses.Manage", "Licenses.Manage" },
                { 69, "Allows Accessories.View", "Accessories.View" },
                { 70, "Allows Accessories.Create", "Accessories.Create" },
                { 71, "Allows Accessories.Update", "Accessories.Update" },
                { 72, "Allows Accessories.Delete", "Accessories.Delete" },
                { 73, "Allows Accessories.Manage", "Accessories.Manage" },
                { 74, "Allows Components.View", "Components.View" },
                { 75, "Allows Components.Create", "Components.Create" },
                { 76, "Allows Components.Update", "Components.Update" },
                { 77, "Allows Components.Delete", "Components.Delete" },
                { 78, "Allows Components.Manage", "Components.Manage" },
                { 79, "Allows Depreciation.View", "Depreciation.View" },
                { 80, "Allows Depreciation.Create", "Depreciation.Create" }
            });

        migrationBuilder.InsertData(
            table: "RolePermissions",
            columns: new[] { "PermissionId", "RoleId" },
            values: new object[,]
            {
                { 26, 10 },
                { 29, 1 },
                { 30, 1 },
                { 31, 1 },
                { 32, 1 },
                { 33, 1 },
                { 34, 1 },
                { 35, 1 },
                { 36, 1 },
                { 37, 1 },
                { 38, 1 },
                { 39, 1 },
                { 40, 1 },
                { 41, 1 },
                { 42, 1 },
                { 43, 1 },
                { 44, 1 },
                { 45, 1 },
                { 46, 1 },
                { 47, 1 },
                { 48, 1 },
                { 49, 1 },
                { 50, 1 },
                { 51, 1 },
                { 52, 1 },
                { 53, 1 },
                { 54, 1 },
                { 55, 1 },
                { 56, 1 },
                { 57, 1 },
                { 58, 1 },
                { 59, 1 },
                { 60, 1 },
                { 61, 1 },
                { 62, 1 },
                { 63, 1 },
                { 64, 1 },
                { 65, 1 },
                { 66, 1 },
                { 67, 1 },
                { 68, 1 },
                { 69, 1 },
                { 70, 1 },
                { 71, 1 },
                { 72, 1 },
                { 73, 1 },
                { 74, 1 },
                { 75, 1 },
                { 76, 1 },
                { 77, 1 },
                { 78, 1 },
                { 79, 1 },
                { 80, 1 },
                { 63, 5 },
                { 68, 5 },
                { 73, 5 },
                { 78, 5 }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 29, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 30, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 31, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 32, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 33, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 34, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 35, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 36, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 37, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 38, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 39, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 40, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 41, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 42, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 43, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 44, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 45, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 46, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 47, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 48, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 49, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 50, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 51, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 52, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 53, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 54, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 55, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 56, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 57, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 58, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 59, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 60, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 61, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 62, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 63, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 64, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 65, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 66, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 67, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 68, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 69, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 70, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 71, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 72, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 73, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 74, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 75, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 76, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 77, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 78, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 79, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 80, 1 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 63, 5 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 68, 5 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 73, 5 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 78, 5 });

        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { 26, 10 });

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 29);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 30);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 31);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 32);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 33);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 34);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 35);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 36);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 37);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 38);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 39);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 40);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 41);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 42);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 43);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 44);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 45);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 46);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 47);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 48);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 49);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 50);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 51);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 52);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 53);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 54);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 55);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 56);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 57);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 58);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 59);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 60);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 61);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 62);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 63);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 64);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 65);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 66);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 67);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 68);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 69);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 70);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 71);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 72);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 73);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 74);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 75);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 76);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 77);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 78);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 79);

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 80);

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 20,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Settings.Manage", "Settings.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 21,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows API.Manage", "API.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 22,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Notifications.Manage", "Notifications.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 23,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Maintenance.Manage", "Maintenance.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 24,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows AuditLogs.View", "AuditLogs.View" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 25,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Licenses.Manage", "Licenses.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 26,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Accessories.Manage", "Accessories.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 27,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Components.Manage", "Components.Manage" });

        migrationBuilder.UpdateData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: 28,
            columns: new[] { "Description", "Name" },
            values: new object[] { "Allows Consumables.Manage", "Consumables.Manage" });

        migrationBuilder.InsertData(
            table: "RolePermissions",
            columns: new[] { "PermissionId", "RoleId" },
            values: new object[] { 23, 10 });
    }
}
