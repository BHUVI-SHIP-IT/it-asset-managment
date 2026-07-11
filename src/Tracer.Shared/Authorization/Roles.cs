namespace Tracer.Shared.Authorization;

/// <summary>
/// Canonical role names used in seed data, JWT claims, and authorization checks.
/// </summary>
public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string SystemAdmin = "SystemAdmin";
    public const string ITAdmin = "ITAdmin";
    public const string AssetManager = "AssetManager";
    public const string InventoryManager = "InventoryManager";
    public const string ProcurementOfficer = "ProcurementOfficer";
    public const string DepartmentManager = "DepartmentManager";
    public const string FinanceOfficer = "FinanceOfficer";
    public const string Auditor = "Auditor";
    public const string HelpDesk = "HelpDesk";
    public const string Employee = "Employee";
    public const string Guest = "Guest";
}
