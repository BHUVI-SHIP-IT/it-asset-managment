import { Routes } from '@angular/router';
import { LayoutComponent } from './core/layout/layout.component';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  // Public routes
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },

  // Protected routes (behind layout + auth guard)
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'assets',
        loadChildren: () => import('./features/assets/assets.routes').then(m => m.ASSETS_ROUTES)
      },
      {
        path: 'users',
        loadChildren: () => import('./features/users/users.routes').then(m => m.USERS_ROUTES)
      },
      {
        path: 'my-items',
        loadComponent: () => import('./features/my-items/my-items.component').then(m => m.MyItemsComponent)
      },
      {
        path: 'requests',
        loadChildren: () => import('./features/requests/requests.routes').then(m => m.REQUESTS_ROUTES)
      },
      {
        path: 'master-data',
        loadChildren: () => import('./features/master-data/master-data.routes').then(m => m.routes)
      },
      {
        path: 'inventory',
        loadChildren: () => import('./features/inventory/inventory.routes').then(m => m.routes)
      },
      {
        path: 'financials',
        loadChildren: () => import('./features/financials/financials.routes').then(m => m.routes)
      },
      {
        path: 'settings',
        loadChildren: () => import('./features/settings/settings.routes').then(m => m.routes)
      }
    ]
  },

  // Fallback
  { path: '**', redirectTo: '' }
];
