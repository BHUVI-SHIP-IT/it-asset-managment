import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'consumables',
    loadComponent: () => import('./consumables/consumable-list/consumable-list.component').then(m => m.ConsumableListComponent)
  },
  {
    path: 'licenses',
    loadComponent: () => import('./licenses/license-list/license-list.component').then(m => m.LicenseListComponent)
  },
  {
    path: 'components',
    loadComponent: () => import('./components/component-list/component-list.component').then(m => m.ComponentListComponent)
  },
  {
    path: 'accessories',
    loadComponent: () => import('./accessories/accessory-list/accessory-list.component').then(m => m.AccessoryListComponent)
  },
  { path: '', redirectTo: 'consumables', pathMatch: 'full' }
];
