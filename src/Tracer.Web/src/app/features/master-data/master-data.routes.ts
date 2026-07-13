import { Routes, Router } from '@angular/router';
import { inject } from '@angular/core';
import { MasterDataComponent } from './master-data.component';
import { permissionGuard } from '../../core/auth/permission.guard';
import { AuthService } from '../../core/auth/auth.service';
import { Permissions } from '../../core/auth/permissions';

const MASTER_DATA_DEFAULTS: { path: string; permission: string }[] = [
  { path: 'categories', permission: Permissions.Categories.View },
  { path: 'locations', permission: Permissions.Locations.View },
  { path: 'companies', permission: Permissions.Settings.View },
  { path: 'departments', permission: Permissions.Departments.View },
  { path: 'manufacturers', permission: Permissions.Manufacturers.View },
  { path: 'asset-models', permission: Permissions.AssetModels.View },
  { path: 'suppliers', permission: Permissions.Suppliers.View },
  { path: 'status-labels', permission: Permissions.StatusLabels.View },
];

export const routes: Routes = [
  {
    path: '',
    component: MasterDataComponent,
    children: [
      {
        path: 'categories',
        canActivate: [permissionGuard(Permissions.Categories.View)],
        loadComponent: () => import('./categories/category-list/category-list.component').then(c => c.CategoryListComponent)
      },
      {
        path: 'locations',
        canActivate: [permissionGuard(Permissions.Locations.View)],
        loadComponent: () => import('./locations/location-list/location-list.component').then(c => c.LocationListComponent)
      },
      {
        path: 'companies',
        canActivate: [permissionGuard(Permissions.Settings.View)],
        loadComponent: () => import('./companies/company-list/company-list.component').then(c => c.CompanyListComponent)
      },
      {
        path: 'manufacturers',
        canActivate: [permissionGuard(Permissions.Manufacturers.View)],
        loadComponent: () => import('./manufacturers/manufacturer-list/manufacturer-list.component').then(c => c.ManufacturerListComponent)
      },
      {
        path: 'suppliers',
        canActivate: [permissionGuard(Permissions.Suppliers.View)],
        loadComponent: () => import('./suppliers/supplier-list/supplier-list.component').then(c => c.SupplierListComponent)
      },
      {
        path: 'status-labels',
        canActivate: [permissionGuard(Permissions.StatusLabels.View)],
        loadComponent: () => import('./status-labels/status-label-list/status-label-list.component').then(c => c.StatusLabelListComponent)
      },
      {
        path: 'departments',
        canActivate: [permissionGuard(Permissions.Departments.View)],
        loadComponent: () => import('./departments/department-list/department-list.component').then(c => c.DepartmentListComponent)
      },
      {
        path: 'asset-models',
        canActivate: [permissionGuard(Permissions.AssetModels.View)],
        loadComponent: () => import('./asset-models/asset-model-list/asset-model-list.component').then(c => c.AssetModelListComponent)
      },
      {
        path: '',
        pathMatch: 'full',
        canActivate: [
          () => {
            const auth = inject(AuthService);
            const router = inject(Router);
            const first = MASTER_DATA_DEFAULTS.find(t => auth.hasPermission(t.permission));
            return router.createUrlTree(
              first ? ['/master-data', first.path] : ['/not-authorized']
            );
          }
        ],
        // Dummy — canActivate always redirects before the component loads.
        loadComponent: () => import('./categories/category-list/category-list.component').then(c => c.CategoryListComponent)
      }
    ]
  }
];
