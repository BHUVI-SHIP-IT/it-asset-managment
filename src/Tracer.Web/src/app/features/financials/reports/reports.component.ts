import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { FinancialsService, ReportExportDto } from '../../../core/services/financials';
import { ToastService } from '../../../core/ui/toast.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
  ],
  template: `
    <div class="header">
      <h2>Financial Reports</h2>
      <button mat-raised-button color="primary" (click)="generateReport('Assets')" [disabled]="generating">
        <mat-icon>assessment</mat-icon> Generate Assets Report
      </button>
    </div>

    <table mat-table [dataSource]="reports()" class="mat-elevation-z8">
      <ng-container matColumnDef="type">
        <th mat-header-cell *matHeaderCellDef> Type </th>
        <td mat-cell *matCellDef="let element"> {{element.reportType}} </td>
      </ng-container>

      <ng-container matColumnDef="requestedAt">
        <th mat-header-cell *matHeaderCellDef> Requested At </th>
        <td mat-cell *matCellDef="let element"> {{element.requestedAt | date:'medium'}} </td>
      </ng-container>

      <ng-container matColumnDef="status">
        <th mat-header-cell *matHeaderCellDef> Status </th>
        <td mat-cell *matCellDef="let element">
          <mat-chip [ngClass]="statusClass(element.status)" selected>
            {{element.status}}
          </mat-chip>
        </td>
      </ng-container>

      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef> Actions </th>
        <td mat-cell *matCellDef="let element">
          <button mat-icon-button color="primary" [disabled]="element.status !== 'Completed'" (click)="download(element.id)">
            <mat-icon>download</mat-icon>
          </button>
        </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </table>
  `,
  styles: [`
    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }
    table {
      width: 100%;
    }
    .status-completed {
      background: #e8f5e9 !important;
      color: #2e7d32 !important;
    }
    .status-pending {
      background: #fff8e1 !important;
      color: #f57f17 !important;
    }
    .status-failed {
      background: #ffebee !important;
      color: #c62828 !important;
    }
  `]
})
export class ReportsComponent implements OnInit, OnDestroy {
  private financialsService = inject(FinancialsService);
  private toast = inject(ToastService);

  reports = signal<ReportExportDto[]>([]);
  displayedColumns: string[] = ['type', 'requestedAt', 'status', 'actions'];
  generating = false;

  private pollTimer: ReturnType<typeof setInterval> | null = null;
  private pollAttempts = 0;
  private readonly maxPollAttempts = 30;

  ngOnInit() {
    this.loadReports();
  }

  ngOnDestroy() {
    this.stopPolling();
  }

  loadReports() {
    this.financialsService.getReports().subscribe({
      next: data => {
        this.reports.set(data ?? []);
        this.syncPolling();
      },
      error: () => {
        this.toast.showError('Failed to load reports');
      }
    });
  }

  generateReport(type: string) {
    this.generating = true;
    this.financialsService.requestReport(type).subscribe({
      next: () => {
        this.generating = false;
        this.toast.showSuccess('Report queued');
        this.pollAttempts = 0;
        this.loadReports();
      },
      error: err => {
        this.generating = false;
        const detail = err?.error?.detail || 'Failed to start report';
        this.toast.showError(detail);
      }
    });
  }

  download(id: string) {
    this.financialsService.downloadReport(id).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `report-${id}.csv`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.toast.showError('Download failed — report may still be processing');
      }
    });
  }

  statusClass(status: string): string {
    switch (status) {
      case 'Completed':
        return 'status-completed';
      case 'Failed':
        return 'status-failed';
      default:
        return 'status-pending';
    }
  }

  private syncPolling() {
    const hasPending = this.reports().some(r => r.status === 'Pending');
    if (!hasPending) {
      this.stopPolling();
      return;
    }
    this.startPolling();
  }

  private startPolling() {
    if (this.pollTimer) {
      return;
    }
    this.pollTimer = setInterval(() => {
      this.pollAttempts += 1;
      if (this.pollAttempts > this.maxPollAttempts) {
        this.stopPolling();
        this.toast.showInfo('Report is still pending — refresh later');
        return;
      }
      this.financialsService.getReports().subscribe({
        next: data => {
          this.reports.set(data ?? []);
          if (!this.reports().some(r => r.status === 'Pending')) {
            this.stopPolling();
          }
        },
        error: () => {
          // Keep polling; transient errors shouldn't kill the wait.
        }
      });
    }, 2000);
  }

  private stopPolling() {
    if (this.pollTimer) {
      clearInterval(this.pollTimer);
      this.pollTimer = null;
    }
    this.pollAttempts = 0;
  }
}
