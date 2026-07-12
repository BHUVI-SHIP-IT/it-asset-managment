import { Routes } from '@angular/router';

export const USERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./user-list/user-list.component').then(c => c.UserListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./user-detail/user-detail.component').then(c => c.UserDetailComponent)
  }
];
