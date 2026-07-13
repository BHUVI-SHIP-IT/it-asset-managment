import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable, map } from 'rxjs';
import { ConfirmDialogComponent, ConfirmDialogData } from './confirm-dialog.component';

/**
 * Opens the shared Material confirm dialog and emits true only when the user confirms.
 */
@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  private readonly dialog = inject(MatDialog);

  open(data: ConfirmDialogData): Observable<boolean> {
    return this.dialog
      .open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
        width: '420px',
        autoFocus: 'dialog',
        data: {
          confirmText: 'Delete',
          cancelText: 'Cancel',
          destructive: true,
          ...data
        }
      })
      .afterClosed()
      .pipe(map(result => result === true));
  }
}
