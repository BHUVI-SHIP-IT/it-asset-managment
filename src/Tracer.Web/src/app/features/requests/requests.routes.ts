import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';
import { Permissions } from '../../core/auth/permissions';

export const REQUESTS_ROUTES: Routes = [
  {
    path: 'mine',
    canActivate: [permissionGuard(Permissions.Requests.ViewOwn)],
    loadComponent: () =>
      import('./my-requests/my-requests.component').then(c => c.MyRequestsComponent)
  },
  {
    path: 'approvals',
    canActivate: [permissionGuard(Permissions.Requests.ViewAll)],
    loadComponent: () =>
      import('./approval-queue/approval-queue.component').then(c => c.ApprovalQueueComponent)
  }
];
