/**
 * Mirror of Tracer.Shared.Authorization.Permissions — keep in sync with
 * src/Tracer.Shared/Authorization/Permissions.cs
 */
export const Permissions = {
  Assets: {
    View: 'Assets.View',
    Create: 'Assets.Create',
    Edit: 'Assets.Edit',
    Delete: 'Assets.Delete',
    Assign: 'Assets.Assign',
    CheckOut: 'Assets.CheckOut',
    CheckIn: 'Assets.CheckIn',
    Transfer: 'Assets.Transfer',
    Clone: 'Assets.Clone',
    Dispose: 'Assets.Dispose',
    Archive: 'Assets.Archive',
  },
  Users: {
    View: 'Users.View',
    Create: 'Users.Create',
    Edit: 'Users.Edit',
    Delete: 'Users.Delete',
  },
  RoleManagement: {
    Manage: 'Roles.Manage',
  },
  PermissionsMgmt: {
    Manage: 'Permissions.Manage',
  },
  Reports: {
    View: 'Reports.View',
    Export: 'Reports.Export',
  },
  Settings: {
    View: 'Settings.View',
    Manage: 'Settings.Manage',
  },
  Api: {
    Manage: 'API.Manage',
  },
  Notifications: {
    View: 'Notifications.View',
    Delete: 'Notifications.Delete',
    Manage: 'Notifications.Manage',
  },
  Maintenance: {
    Manage: 'Maintenance.Manage',
  },
  AuditLogs: {
    View: 'AuditLogs.View',
  },
  Categories: {
    View: 'Categories.View',
    Create: 'Categories.Create',
    Update: 'Categories.Update',
    Delete: 'Categories.Delete',
  },
  Locations: {
    View: 'Locations.View',
    Create: 'Locations.Create',
    Update: 'Locations.Update',
    Delete: 'Locations.Delete',
  },
  Departments: {
    View: 'Departments.View',
    Create: 'Departments.Create',
    Update: 'Departments.Update',
    Delete: 'Departments.Delete',
  },
  Manufacturers: {
    View: 'Manufacturers.View',
    Create: 'Manufacturers.Create',
    Update: 'Manufacturers.Update',
    Delete: 'Manufacturers.Delete',
  },
  Suppliers: {
    View: 'Suppliers.View',
    Create: 'Suppliers.Create',
    Update: 'Suppliers.Update',
    Delete: 'Suppliers.Delete',
  },
  StatusLabels: {
    View: 'StatusLabels.View',
    Create: 'StatusLabels.Create',
    Update: 'StatusLabels.Update',
    Delete: 'StatusLabels.Delete',
  },
  AssetModels: {
    View: 'AssetModels.View',
    Create: 'AssetModels.Create',
    Update: 'AssetModels.Update',
    Delete: 'AssetModels.Delete',
  },
  CustomFields: {
    View: 'CustomFields.View',
    Manage: 'CustomFields.Manage',
  },
  Consumables: {
    View: 'Consumables.View',
    Create: 'Consumables.Create',
    Update: 'Consumables.Update',
    Delete: 'Consumables.Delete',
    Checkout: 'Consumables.Checkout',
    Manage: 'Consumables.Manage',
  },
  Licenses: {
    View: 'Licenses.View',
    Create: 'Licenses.Create',
    Update: 'Licenses.Update',
    Delete: 'Licenses.Delete',
    Manage: 'Licenses.Manage',
  },
  Accessories: {
    View: 'Accessories.View',
    Create: 'Accessories.Create',
    Update: 'Accessories.Update',
    Delete: 'Accessories.Delete',
    Manage: 'Accessories.Manage',
  },
  Components: {
    View: 'Components.View',
    Create: 'Components.Create',
    Update: 'Components.Update',
    Delete: 'Components.Delete',
    Manage: 'Components.Manage',
  },
  Depreciation: {
    View: 'Depreciation.View',
    Create: 'Depreciation.Create',
  },
  Requests: {
    Create: 'Requests.Create',
    ViewOwn: 'Requests.ViewOwn',
    ViewAll: 'Requests.ViewAll',
    Approve: 'Requests.Approve',
  },
} as const;

export function getManagePermission(permission: string): string | null {
  const dot = permission.lastIndexOf('.');
  if (dot <= 0) return null;
  return `${permission.slice(0, dot)}.Manage`;
}

export function satisfiesPermission(userPermissions: readonly string[], required: string): boolean {
  if (userPermissions.includes(required)) return true;
  const manage = getManagePermission(required);
  return manage !== null && userPermissions.includes(manage);
}
