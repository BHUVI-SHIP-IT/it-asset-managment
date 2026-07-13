import { Permissions } from '../auth/permissions';

export interface NavItem {
  label: string;
  route: string;
  icon: string;
  /** Single permission or any-of list required to show this item. */
  permission?: string | readonly string[];
}

export const NAV_ITEMS: NavItem[] = [
  { label: 'Dashboard', route: '/dashboard', icon: 'dashboard' },
  { label: 'My Items', route: '/my-items', icon: 'inventory' },
  { label: 'My Requests', route: '/requests/mine', icon: 'assignment', permission: Permissions.Requests.ViewOwn },
  { label: 'Approval Queue', route: '/requests/approvals', icon: 'rule', permission: Permissions.Requests.ViewAll },
  { label: 'Assets', route: '/assets', icon: 'devices', permission: Permissions.Assets.View },
  { label: 'Users', route: '/users', icon: 'group', permission: Permissions.Users.View },
  {
    label: 'Master Data',
    route: '/master-data',
    icon: 'category',
    permission: [
      Permissions.Categories.View,
      Permissions.Locations.View,
      Permissions.Departments.View,
      Permissions.Manufacturers.View,
      Permissions.Suppliers.View,
      Permissions.StatusLabels.View,
      Permissions.AssetModels.View,
      Permissions.Settings.View,
    ],
  },
  { label: 'Consumables', route: '/inventory/consumables', icon: 'inventory_2', permission: Permissions.Consumables.View },
  { label: 'Components', route: '/inventory/components', icon: 'memory', permission: Permissions.Components.View },
  { label: 'Accessories', route: '/inventory/accessories', icon: 'headset', permission: Permissions.Accessories.View },
  { label: 'Licenses', route: '/inventory/licenses', icon: 'vpn_key', permission: Permissions.Licenses.View },
  { label: 'Depreciation', route: '/financials/depreciation', icon: 'trending_down', permission: Permissions.Depreciation.View },
  { label: 'Reports', route: '/financials/reports', icon: 'assessment', permission: Permissions.Reports.View },
  { label: 'Settings', route: '/settings', icon: 'settings', permission: Permissions.Settings.View },
  { label: 'Custom Fields', route: '/settings/custom-fields', icon: 'text_fields', permission: Permissions.CustomFields.View },
];
