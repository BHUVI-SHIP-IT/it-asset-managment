import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { UserDashboardComponent } from './user-dashboard.component';

/**
 * Role-based dashboard host.
 * Mounts either the admin or user dashboard component — never both —
 * so org-wide metrics are never requested for Employee/Guest tokens.
 */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [AdminDashboardComponent, UserDashboardComponent],
  template: `
    @if (auth.isAdminDashboardUser()) {
      <app-admin-dashboard />
    } @else {
      <app-user-dashboard />
    }
  `
})
export class DashboardComponent {
  readonly auth = inject(AuthService);
}
