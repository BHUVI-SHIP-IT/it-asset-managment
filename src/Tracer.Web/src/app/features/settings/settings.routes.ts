import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./settings.component').then(c => c.SettingsComponent)
  },
  {
    path: 'custom-fields',
    loadComponent: () => import('./custom-fields/custom-field-list.component').then(c => c.CustomFieldListComponent)
  }
];
