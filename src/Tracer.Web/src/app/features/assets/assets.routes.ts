import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';
import { Permissions } from '../../core/auth/permissions';

export const ASSETS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [permissionGuard(Permissions.Assets.View)],
    loadComponent: () => import('./asset-list/asset-list.component').then(c => c.AssetListComponent)
  },
  {
    path: 'new',
    canActivate: [permissionGuard(Permissions.Assets.Create)],
    loadComponent: () => import('./asset-form/asset-form.component').then(c => c.AssetFormComponent)
  },
  {
    path: ':id/edit',
    canActivate: [permissionGuard(Permissions.Assets.Edit)],
    loadComponent: () => import('./asset-form/asset-form.component').then(c => c.AssetFormComponent)
  },
  {
    path: ':id',
    canActivate: [permissionGuard(Permissions.Assets.View)],
    loadComponent: () => import('./asset-detail/asset-detail.component').then(c => c.AssetDetailComponent)
  }
];
