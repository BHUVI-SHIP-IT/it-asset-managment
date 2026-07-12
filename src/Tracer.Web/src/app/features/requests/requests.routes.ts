import { Routes } from '@angular/router';
import { Permissions } from '../../core/auth/permissions';

export const REQUESTS_ROUTES: Routes = [
  {
    path: 'mine',
    loadComponent: () =>
      import('./my-requests/my-requests.component').then(c => c.MyRequestsComponent)
  },
  {
    path: 'approvals',
    loadComponent: () =>
      import('./approval-queue/approval-queue.component').then(c => c.ApprovalQueueComponent)
  }
];

export { Permissions };
