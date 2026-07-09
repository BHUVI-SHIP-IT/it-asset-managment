import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { FinancialsService, ReportExportDto } from '../../../core/services/financials';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatChipsModule],
  template: `
    <div class="header">
      <h2>Financial Reports</h2>
      <button mat-raised-button color="primary" (click)="generateReport('Assets')">
        <mat-icon>assessment</mat-icon> Generate Assets Report
      </button>
    </div>

    <table mat-table [dataSource]="reports" class="mat-elevation-z8">
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
          <mat-chip [color]="element.status === 'Completed' ? 'primary' : 'accent'" selected>
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
  `]
})
export class ReportsComponent implements OnInit {
  private financialsService = inject(FinancialsService);

  reports: ReportExportDto[] = [];
  displayedColumns: string[] = ['type', 'requestedAt', 'status', 'actions'];

  ngOnInit() {
    this.loadReports();
  }

  loadReports() {
    this.financialsService.getReports().subscribe(data => {
      this.reports = data;
    });
  }

  generateReport(type: string) {
    this.financialsService.requestReport(type).subscribe(() => {
      this.loadReports(); // Refresh the list
    });
  }

  download(id: string) {
    this.financialsService.downloadReport(id).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `report-${id}.csv`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }
}
