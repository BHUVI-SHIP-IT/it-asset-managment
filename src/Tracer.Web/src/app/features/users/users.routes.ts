import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';
import { Permissions } from '../../core/auth/permissions';

export const USERS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [permissionGuard(Permissions.Users.View)],
    loadComponent: () => import('./user-list/user-list.component').then(c => c.UserListComponent)
  },
  {
    path: ':id',
    canActivate: [permissionGuard(Permissions.Users.View)],
    loadComponent: () => import('./user-detail/user-detail.component').then(c => c.UserDetailComponent)
  }
];
