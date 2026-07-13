import { Component, computed, inject } from '@angular/core';
import { Router, RouterOutlet, RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../auth/auth.service';
import { satisfiesAnyPermission } from '../auth/permissions';
import { AlertsComponent } from '../notifications/alerts.component';
import { NAV_ITEMS } from './nav-items';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    AlertsComponent
  ],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {
  authService = inject(AuthService);
  private router = inject(Router);

  /** Only nav items the current user is allowed to open. */
  navItems = computed(() => {
    const perms = this.authService.permissions();
    return NAV_ITEMS.filter(
      item => !item.permission || satisfiesAnyPermission(perms, item.permission)
    );
  });

  logout() {
    this.authService.logout();
    // Click handlers run inside NgZone — navigate here so the layout shell
    // unmounts immediately (auth guard does not re-run on signal clear alone).
    void this.router.navigateByUrl('/login', { replaceUrl: true });
  }
}
