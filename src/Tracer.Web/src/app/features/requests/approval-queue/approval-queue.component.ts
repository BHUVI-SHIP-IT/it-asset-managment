import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Permissions } from '../../../core/auth/permissions';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { RequestDto, RequestService } from '../request.service';

@Component({
  selector: 'app-approval-queue',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatTabsModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    HasPermissionDirective
  ],
  templateUrl: './approval-queue.component.html',
  styleUrls: ['./approval-queue.component.scss']
})
export class ApprovalQueueComponent implements OnInit {
  readonly permissions = Permissions;
  private requestService = inject(RequestService);
  private snackBar = inject(MatSnackBar);

  loading = signal(true);
  pending = signal<RequestDto[]>([]);
  history = signal<RequestDto[]>([]);
  pendingColumns = ['requestedByName', 'type', 'itemName', 'quantity', 'requestedAtUtc', 'notes', 'actions'];
  historyColumns = ['requestedByName', 'type', 'itemName', 'status', 'resolvedByName', 'resolvedAtUtc'];

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.loading.set(true);
    this.requestService.getAll().subscribe({
      next: rows => {
        this.pending.set(rows.filter(r => r.status === 'Pending'));
        this.history.set(rows.filter(r => r.status !== 'Pending'));
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Failed to load approval queue', 'Close', { duration: 4000 });
        this.loading.set(false);
      }
    });
  }

  approve(row: RequestDto): void {
    this.requestService.approve(row.id).subscribe({
      next: () => {
        this.snackBar.open('Request approved', 'Close', { duration: 3000 });
        this.reload();
      },
      error: err => {
        this.snackBar.open(err?.error?.detail || 'Approve failed', 'Close', { duration: 5000 });
      }
    });
  }

  reject(row: RequestDto): void {
    this.requestService.reject(row.id).subscribe({
      next: () => {
        this.snackBar.open('Request rejected', 'Close', { duration: 3000 });
        this.reload();
      },
      error: err => {
        this.snackBar.open(err?.error?.detail || 'Reject failed', 'Close', { duration: 5000 });
      }
    });
  }
}
