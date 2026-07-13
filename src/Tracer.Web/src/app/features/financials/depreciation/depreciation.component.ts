import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FinancialsService, DepreciationDto } from '../../../core/services/financials';

@Component({
  selector: 'app-depreciation',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule],
  template: `
    <div class="header">
      <h2>Depreciation Schedules</h2>
    </div>

    <table mat-table [dataSource]="schedules()" class="mat-elevation-z8">
      <ng-container matColumnDef="name">
        <th mat-header-cell *matHeaderCellDef> Name </th>
        <td mat-cell *matCellDef="let element"> {{element.name}} </td>
      </ng-container>

      <ng-container matColumnDef="months">
        <th mat-header-cell *matHeaderCellDef> Months </th>
        <td mat-cell *matCellDef="let element"> {{element.months}} </td>
      </ng-container>

      <ng-container matColumnDef="minimumValue">
        <th mat-header-cell *matHeaderCellDef> Minimum Value </th>
        <td mat-cell *matCellDef="let element"> {{element.minimumValue | currency}} </td>
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
export class DepreciationComponent implements OnInit {
  private financialsService = inject(FinancialsService);

  schedules = signal<DepreciationDto[]>([]);
  displayedColumns: string[] = ['name', 'months', 'minimumValue'];

  ngOnInit() {
    this.loadSchedules();
  }

  loadSchedules() {
    this.financialsService.getDepreciationSchedules().subscribe({
      next: data => this.schedules.set(data),
      error: () => this.schedules.set([])
    });
  }
}
