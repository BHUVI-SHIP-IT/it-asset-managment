import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Permissions } from '../../../core/auth/permissions';
import { ToastService } from '../../../core/ui/toast.service';
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
    HasPermissionDirective
  ],
  templateUrl: './approval-queue.component.html',
  styleUrls: ['./approval-queue.component.scss']
})
export class ApprovalQueueComponent implements OnInit {
  readonly permissions = Permissions;
  private requestService = inject(RequestService);
  private toast = inject(ToastService);

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
        this.toast.showError('Failed to load approval queue');
        this.loading.set(false);
      }
    });
  }

  approve(row: RequestDto): void {
    this.requestService.approve(row.id).subscribe({
      next: () => {
        this.toast.showSuccess('Request approved');
        this.reload();
      },
      error: err => {
        this.toast.showError(err?.error?.detail || 'Approve failed');
      }
    });
  }

  reject(row: RequestDto): void {
    this.requestService.reject(row.id).subscribe({
      next: () => {
        this.toast.showSuccess('Request rejected');
        this.reload();
      },
      error: err => {
        this.toast.showError(err?.error?.detail || 'Reject failed');
      }
    });
  }
}
