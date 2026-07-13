import { Injectable, inject } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

/**
 * App-wide toast notifications wrapping MatSnackBar.
 * Named ToastService (not NotificationService) to avoid colliding with the
 * in-app alerts API client at core/notifications/notification.service.ts.
 */
@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly snackBar = inject(MatSnackBar);

  showSuccess(message: string): void {
    this.open(message, {
      duration: 3000,
      panelClass: ['notification-snackbar', 'notification-success']
    });
  }

  showError(message: string): void {
    this.open(message, {
      duration: 5000,
      panelClass: ['notification-snackbar', 'notification-error']
    });
  }

  showInfo(message: string): void {
    this.open(message, {
      duration: 4000,
      panelClass: ['notification-snackbar', 'notification-info']
    });
  }

  private open(message: string, overrides: MatSnackBarConfig): void {
    this.snackBar.open(message, 'Close', {
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      ...overrides
    });
  }
}
