import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';
import { Permissions } from '../../core/auth/permissions';

export const routes: Routes = [
  {
    path: '',
    canActivate: [permissionGuard(Permissions.Settings.View)],
    loadComponent: () => import('./settings.component').then(c => c.SettingsComponent)
  },
  {
    path: 'custom-fields',
    canActivate: [permissionGuard(Permissions.CustomFields.View)],
    loadComponent: () => import('./custom-fields/custom-field-list.component').then(c => c.CustomFieldListComponent)
  }
];
