import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../auth/auth.service';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';
import { AlertsComponent } from '../notifications/alerts.component';

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
    HasPermissionDirective,
    AlertsComponent
  ],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {
  authService = inject(AuthService);

  logout() {
    this.authService.logout();
  }
}
