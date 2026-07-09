import { Routes } from '@angular/router';

export const ASSETS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./asset-list/asset-list.component').then(c => c.AssetListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./asset-form/asset-form.component').then(c => c.AssetFormComponent)
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./asset-form/asset-form.component').then(c => c.AssetFormComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./asset-detail/asset-detail.component').then(c => c.AssetDetailComponent)
  }
];
