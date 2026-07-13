import { Routes, Router } from '@angular/router';
import { inject } from '@angular/core';
import { permissionGuard } from '../../core/auth/permission.guard';
import { AuthService } from '../../core/auth/auth.service';
import { Permissions } from '../../core/auth/permissions';

const INVENTORY_DEFAULTS: { path: string; permission: string }[] = [
  { path: 'consumables', permission: Permissions.Consumables.View },
  { path: 'components', permission: Permissions.Components.View },
  { path: 'accessories', permission: Permissions.Accessories.View },
  { path: 'licenses', permission: Permissions.Licenses.View },
];

export const routes: Routes = [
  {
    path: 'consumables',
    canActivate: [permissionGuard(Permissions.Consumables.View)],
    loadComponent: () => import('./consumables/consumable-list/consumable-list.component').then(m => m.ConsumableListComponent)
  },
  {
    path: 'licenses',
    canActivate: [permissionGuard(Permissions.Licenses.View)],
    loadComponent: () => import('./licenses/license-list/license-list.component').then(m => m.LicenseListComponent)
  },
  {
    path: 'components',
    canActivate: [permissionGuard(Permissions.Components.View)],
    loadComponent: () => import('./components/component-list/component-list.component').then(m => m.ComponentListComponent)
  },
  {
    path: 'accessories',
    canActivate: [permissionGuard(Permissions.Accessories.View)],
    loadComponent: () => import('./accessories/accessory-list/accessory-list.component').then(m => m.AccessoryListComponent)
  },
  {
    path: '',
    pathMatch: 'full',
    canActivate: [
      () => {
        const auth = inject(AuthService);
        const router = inject(Router);
        const first = INVENTORY_DEFAULTS.find(t => auth.hasPermission(t.permission));
        return router.createUrlTree(
          first ? ['/inventory', first.path] : ['/not-authorized']
        );
      }
    ],
    // Dummy — canActivate always redirects before the component loads.
    loadComponent: () =>
      import('./consumables/consumable-list/consumable-list.component').then(m => m.ConsumableListComponent)
  }
];
