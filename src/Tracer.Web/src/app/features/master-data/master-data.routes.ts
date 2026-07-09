import { Routes } from '@angular/router';
import { MasterDataComponent } from './master-data.component';

export const routes: Routes = [
  {
    path: '',
    component: MasterDataComponent,
    children: [
      {
        path: 'categories',
        loadComponent: () => import('./categories/category-list/category-list.component').then(c => c.CategoryListComponent)
      },
      {
        path: 'locations',
        loadComponent: () => import('./locations/location-list/location-list.component').then(c => c.LocationListComponent)
      },
      {
        path: 'companies',
        loadComponent: () => import('./companies/company-list/company-list.component').then(c => c.CompanyListComponent)
      },
      {
        path: 'manufacturers',
        loadComponent: () => import('./manufacturers/manufacturer-list/manufacturer-list.component').then(c => c.ManufacturerListComponent)
      },
      {
        path: 'suppliers',
        loadComponent: () => import('./suppliers/supplier-list/supplier-list.component').then(c => c.SupplierListComponent)
      },
      {
        path: 'status-labels',
        loadComponent: () => import('./status-labels/status-label-list/status-label-list.component').then(c => c.StatusLabelListComponent)
      },
      {
        path: 'departments',
        loadComponent: () => import('./departments/department-list/department-list.component').then(c => c.DepartmentListComponent)
      },
      {
        path: 'asset-models',
        loadComponent: () => import('./asset-models/asset-model-list/asset-model-list.component').then(c => c.AssetModelListComponent)
      },
      {
        path: '',
        redirectTo: 'categories',
        pathMatch: 'full'
      }
    ]
  }
];
