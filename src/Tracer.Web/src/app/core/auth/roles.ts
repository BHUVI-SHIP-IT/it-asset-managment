/**
 * Mirror of Tracer.Shared.Authorization.Roles — keep in sync with
 * src/Tracer.Shared/Authorization/Roles.cs
 */
export const Roles = {
  SuperAdmin: 'SuperAdmin',
  SystemAdmin: 'SystemAdmin',
  ITAdmin: 'ITAdmin',
  AssetManager: 'AssetManager',
  InventoryManager: 'InventoryManager',
  ProcurementOfficer: 'ProcurementOfficer',
  DepartmentManager: 'DepartmentManager',
  FinanceOfficer: 'FinanceOfficer',
  Auditor: 'Auditor',
  HelpDesk: 'HelpDesk',
  Employee: 'Employee',
  Guest: 'Guest',
} as const;

export type RoleName = (typeof Roles)[keyof typeof Roles];

/** Roles that use the personal (non-org-wide) dashboard. */
const END_USER_ROLES = new Set<string>([Roles.Employee, Roles.Guest]);

export function isEndUserRole(role: string | null | undefined): boolean {
  return !role || END_USER_ROLES.has(role);
}
