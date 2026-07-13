import { Routes, Router } from '@angular/router';
import { inject } from '@angular/core';
import { permissionGuard } from '../../core/auth/permission.guard';
import { AuthService } from '../../core/auth/auth.service';
import { Permissions } from '../../core/auth/permissions';

const FINANCIAL_DEFAULTS: { path: string; permission: string }[] = [
  { path: 'reports', permission: Permissions.Reports.View },
  { path: 'depreciation', permission: Permissions.Depreciation.View },
];

export const routes: Routes = [
  {
    path: 'reports',
    canActivate: [permissionGuard(Permissions.Reports.View)],
    loadComponent: () => import('./reports/reports.component').then(m => m.ReportsComponent)
  },
  {
    path: 'depreciation',
    canActivate: [permissionGuard(Permissions.Depreciation.View)],
    loadComponent: () => import('./depreciation/depreciation.component').then(m => m.DepreciationComponent)
  },
  {
    path: '',
    pathMatch: 'full',
    canActivate: [
      () => {
        const auth = inject(AuthService);
        const router = inject(Router);
        const first = FINANCIAL_DEFAULTS.find(t => auth.hasPermission(t.permission));
        return router.createUrlTree(
          first ? ['/financials', first.path] : ['/not-authorized']
        );
      }
    ],
    loadComponent: () => import('./reports/reports.component').then(m => m.ReportsComponent)
  }
];
