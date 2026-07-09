import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'reports',
    loadComponent: () => import('./reports/reports.component').then(m => m.ReportsComponent)
  },
  {
    path: 'depreciation',
    loadComponent: () => import('./depreciation/depreciation.component').then(m => m.DepreciationComponent)
  },
  { path: '', redirectTo: 'reports', pathMatch: 'full' }
];
