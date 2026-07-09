import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDividerModule } from '@angular/material/divider';
import { NotificationService, NotificationDto } from './notification.service';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-alerts',
  standalone: true,
  imports: [
    CommonModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
    MatDividerModule,
    HasPermissionDirective,
    DatePipe
  ],
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.scss']
})
export class AlertsComponent implements OnInit {
  private notificationService = inject(NotificationService);

  notifications = signal<NotificationDto[]>([]);
  unreadCount = signal<number>(0);

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.notificationService.getNotifications(1, 10, false).subscribe({
      next: (items) => {
        const resultItems = Array.isArray(items) ? items : [];
        this.notifications.set(resultItems);
        this.unreadCount.set(resultItems.filter(n => !n.isRead).length);
      },
      error: (err) => console.error('Failed to load notifications', err)
    });
  }

  markAsRead(notification: NotificationDto, event: Event): void {
    event.stopPropagation(); // Keep menu open
    if (notification.isRead) return;

    this.notificationService.markRead(notification.id).subscribe({
      next: () => {
        notification.isRead = true;
        this.unreadCount.update(c => Math.max(0, c - 1));
      }
    });
  }
}
