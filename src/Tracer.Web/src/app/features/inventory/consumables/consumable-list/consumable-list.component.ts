import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { InventoryService, ConsumableDto } from '../../../../core/services/inventory';

@Component({
  selector: 'app-consumable-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatDialogModule],
  template: `
    <div class="header">
      <h2>Consumables</h2>
      <button mat-raised-button color="primary">
        <mat-icon>add</mat-icon> Create Consumable
      </button>
    </div>

    <table mat-table [dataSource]="consumables" class="mat-elevation-z8">
      <ng-container matColumnDef="id">
        <th mat-header-cell *matHeaderCellDef> ID </th>
        <td mat-cell *matCellDef="let element"> {{element.id}} </td>
      </ng-container>

      <ng-container matColumnDef="name">
        <th mat-header-cell *matHeaderCellDef> Name </th>
        <td mat-cell *matCellDef="let element"> {{element.name}} </td>
      </ng-container>

      <ng-container matColumnDef="quantity">
        <th mat-header-cell *matHeaderCellDef> Total Quantity </th>
        <td mat-cell *matCellDef="let element"> {{element.totalQuantity}} </td>
      </ng-container>

      <ng-container matColumnDef="cost">
        <th mat-header-cell *matHeaderCellDef> Purchase Cost </th>
        <td mat-cell *matCellDef="let element"> {{element.purchaseCost | currency}} </td>
      </ng-container>

      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef> Actions </th>
        <td mat-cell *matCellDef="let element">
          <button mat-icon-button color="primary">
            <mat-icon>edit</mat-icon>
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
export class ConsumableListComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private dialog = inject(MatDialog);

  consumables: ConsumableDto[] = [];
  displayedColumns: string[] = ['id', 'name', 'quantity', 'cost', 'actions'];

  ngOnInit() {
    this.loadConsumables();
  }

  loadConsumables() {
    this.inventoryService.getConsumables().subscribe(data => {
      this.consumables = data;
    });
  }
}
