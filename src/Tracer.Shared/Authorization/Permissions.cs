namespace Tracer.Shared.Authorization;

/// <summary>
/// Single source of truth for permission names. Used by seed data, JWT claims,
/// authorization policies, controllers, and the Angular frontend (mirrored in permissions.ts).
/// </summary>
public static class Permissions
{
    public static class Assets
    {
        public const string View = "Assets.View";
        public const string Create = "Assets.Create";
        public const string Edit = "Assets.Edit";
        public const string Delete = "Assets.Delete";
        public const string Assign = "Assets.Assign";
        public const string CheckOut = "Assets.CheckOut";
        public const string CheckIn = "Assets.CheckIn";
        public const string Transfer = "Assets.Transfer";
        public const string Clone = "Assets.Clone";
        public const string Dispose = "Assets.Dispose";
        public const string Archive = "Assets.Archive";

        public static readonly string[] All =
        [
            View, Create, Edit, Delete, Assign, CheckOut, CheckIn, Transfer, Clone, Dispose, Archive
        ];
    }

    public static class Users
    {
        public const string View = "Users.View";
        public const string Create = "Users.Create";
        public const string Edit = "Users.Edit";
        public const string Delete = "Users.Delete";

        public static readonly string[] All = [View, Create, Edit, Delete];
    }

    public static class RoleManagement
    {
        public const string Manage = "Roles.Manage";
        public static readonly string[] All = [Manage];
    }

    public static class PermissionsMgmt
    {
        public const string Manage = "Permissions.Manage";
        public static readonly string[] All = [Manage];
    }

    public static class Reports
    {
        public const string View = "Reports.View";
        public const string Export = "Reports.Export";

        public static readonly string[] All = [View, Export];
    }

    public static class Settings
    {
        public const string View = "Settings.View";
        public const string Manage = "Settings.Manage";

        public static readonly string[] All = [View, Manage];
    }

    public static class Api
    {
        public const string Manage = "API.Manage";
        public static readonly string[] All = [Manage];
    }

    public static class Notifications
    {
        public const string View = "Notifications.View";
        public const string Delete = "Notifications.Delete";
        public const string Manage = "Notifications.Manage";

        public static readonly string[] All = [View, Delete, Manage];
    }

    public static class Maintenance
    {
        public const string Manage = "Maintenance.Manage";
        public static readonly string[] All = [Manage];
    }

    public static class AuditLogs
    {
        public const string View = "AuditLogs.View";
        public static readonly string[] All = [View];
    }

    public static class Categories
    {
        public const string View = "Categories.View";
        public const string Create = "Categories.Create";
        public const string Update = "Categories.Update";
        public const string Delete = "Categories.Delete";

        public static readonly string[] All = [View, Create, Update, Delete];
    }

    public static class Locations
    {
        public const string View = "Locations.View";
        public const string Create = "Locations.Create";
        public const string Update = "Locations.Update";
        public const string Delete = "Locations.Delete";

        public static readonly string[] All = [View, Create, Update, Delete];
    }

    public static class Departments
    {
        public const string View = "Departments.View";
        public const string Create = "Departments.Create";
        public const string Update = "Departments.Update";
        public const string Delete = "Departments.Delete";

        public static readonly string[] All = [View, Create, Update, Delete];
    }

    public static class Manufacturers
    {
        public const string View = "Manufacturers.View";
        public const string Create = "Manufacturers.Create";
        public const string Update = "Manufacturers.Update";
        public const string Delete = "Manufacturers.Delete";

        public static readonly string[] All = [View, Create, Update, Delete];
    }

    public static class Suppliers
    {
        public const string View = "Suppliers.View";
        public const string Create = "Suppliers.Create";
        public const string Update = "Suppliers.Update";
        public const string Delete = "Suppliers.Delete";

        public static readonly string[] All = [View, Create, Update, Delete];
    }

    public static class StatusLabels
    {
        public const string View = "StatusLabels.View";
        public const string Create = "StatusLabels.Create";
        public const string Update = "StatusLabels.Update";
        public const string Delete = "StatusLabels.Delete";

        public static readonly string[] All = [View, Create, Update, Delete];
    }

    public static class AssetModels
    {
        public const string View = "AssetModels.View";
        public const string Create = "AssetModels.Create";
        public const string Update = "AssetModels.Update";
        public const string Delete = "AssetModels.Delete";

        public static readonly string[] All = [View, Create, Update, Delete];
    }

    public static class CustomFields
    {
        public const string View = "CustomFields.View";
        public const string Manage = "CustomFields.Manage";

        public static readonly string[] All = [View, Manage];
    }

    public static class Consumables
    {
        public const string View = "Consumables.View";
        public const string Create = "Consumables.Create";
        public const string Update = "Consumables.Update";
        public const string Delete = "Consumables.Delete";
        public const string Checkout = "Consumables.Checkout";
        public const string Manage = "Consumables.Manage";

        public static readonly string[] All = [View, Create, Update, Delete, Checkout, Manage];
    }

    public static class Licenses
    {
        public const string View = "Licenses.View";
        public const string Create = "Licenses.Create";
        public const string Update = "Licenses.Update";
        public const string Delete = "Licenses.Delete";
        public const string Manage = "Licenses.Manage";

        public static readonly string[] All = [View, Create, Update, Delete, Manage];
    }

    public static class Accessories
    {
        public const string View = "Accessories.View";
        public const string Create = "Accessories.Create";
        public const string Update = "Accessories.Update";
        public const string Delete = "Accessories.Delete";
        public const string Manage = "Accessories.Manage";

        public static readonly string[] All = [View, Create, Update, Delete, Manage];
    }

    public static class Components
    {
        public const string View = "Components.View";
        public const string Create = "Components.Create";
        public const string Update = "Components.Update";
        public const string Delete = "Components.Delete";
        public const string Manage = "Components.Manage";

        public static readonly string[] All = [View, Create, Update, Delete, Manage];
    }

    public static class Depreciation
    {
        public const string View = "Depreciation.View";
        public const string Create = "Depreciation.Create";

        public static readonly string[] All = [View, Create];
    }

    public static class Requests
    {
        public const string Create = "Requests.Create";
        public const string ViewOwn = "Requests.ViewOwn";
        public const string ViewAll = "Requests.ViewAll";
        public const string Approve = "Requests.Approve";

        public static readonly string[] All = [Create, ViewOwn, ViewAll, Approve];
    }

    /// <summary>All permissions in stable seed order (IDs 1..N).</summary>
    public static readonly string[] All =
    [
        ..Assets.All,
        ..Users.All,
        ..RoleManagement.All,
        ..PermissionsMgmt.All,
        ..Reports.All,
        ..Settings.All,
        ..Api.All,
        ..Notifications.All,
        ..Maintenance.All,
        ..AuditLogs.All,
        ..Categories.All,
        ..Locations.All,
        ..Departments.All,
        ..Manufacturers.All,
        ..Suppliers.All,
        ..StatusLabels.All,
        ..AssetModels.All,
        ..CustomFields.All,
        ..Consumables.All,
        ..Licenses.All,
        ..Accessories.All,
        ..Components.All,
        ..Depreciation.All,
        ..Requests.All,
    ];

    /// <summary>
    /// Returns the umbrella Manage permission for a resource (e.g. Consumables.View → Consumables.Manage).
    /// </summary>
    public static string? GetManagePermission(string permission)
    {
        var dot = permission.LastIndexOf('.');
        if (dot <= 0) return null;
        return permission[..dot] + ".Manage";
    }
}
