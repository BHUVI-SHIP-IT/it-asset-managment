import { Component, OnInit, inject } from '@angular/core';
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

    <table mat-table [dataSource]="schedules" class="mat-elevation-z8">
      <ng-container matColumnDef="assetName">
        <th mat-header-cell *matHeaderCellDef> Asset </th>
        <td mat-cell *matCellDef="let element"> {{element.assetName}} </td>
      </ng-container>

      <ng-container matColumnDef="purchaseCost">
        <th mat-header-cell *matHeaderCellDef> Purchase Cost </th>
        <td mat-cell *matCellDef="let element"> {{element.purchaseCost | currency}} </td>
      </ng-container>

      <ng-container matColumnDef="currentValue">
        <th mat-header-cell *matHeaderCellDef> Current Value </th>
        <td mat-cell *matCellDef="let element"> {{element.currentValue | currency}} </td>
      </ng-container>

      <ng-container matColumnDef="monthsElapsed">
        <th mat-header-cell *matHeaderCellDef> Months Elapsed </th>
        <td mat-cell *matCellDef="let element"> {{element.monthsElapsed}} </td>
      </ng-container>

      <ng-container matColumnDef="calculatedAt">
        <th mat-header-cell *matHeaderCellDef> Calculated At </th>
        <td mat-cell *matCellDef="let element"> {{element.calculatedAt | date}} </td>
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

  schedules: DepreciationDto[] = [];
  displayedColumns: string[] = ['assetName', 'purchaseCost', 'currentValue', 'monthsElapsed', 'calculatedAt'];

  ngOnInit() {
    this.loadSchedules();
  }

  loadSchedules() {
    this.financialsService.getDepreciationSchedules().subscribe(data => {
      this.schedules = data;
    });
  }
}
