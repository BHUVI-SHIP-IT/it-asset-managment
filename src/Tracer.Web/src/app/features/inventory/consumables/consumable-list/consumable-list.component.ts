import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { Permissions } from '../../../../core/auth/permissions';
import { ToastService } from '../../../../core/ui/toast.service';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { InventoryService, ConsumableDto } from '../../../../core/services/inventory';
import { ConsumableFormDialogComponent } from '../consumable-form-dialog/consumable-form-dialog.component';

@Component({
  selector: 'app-consumable-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    HasPermissionDirective
  ],
  template: `
    <div class="header">
      <h2>Consumables</h2>
      <button mat-raised-button color="primary"
              *hasPermission="permissions.Consumables.Create"
              (click)="openCreateDialog()">
        <mat-icon>add</mat-icon> Create Consumable
      </button>
    </div>

    <table mat-table [dataSource]="consumables()" class="mat-elevation-z8">
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
  readonly permissions = Permissions;

  private inventoryService = inject(InventoryService);
  private dialog = inject(MatDialog);
  private toast = inject(ToastService);

  consumables = signal<ConsumableDto[]>([]);
  displayedColumns: string[] = ['id', 'name', 'quantity', 'cost'];

  ngOnInit() {
    this.loadConsumables();
  }

  loadConsumables() {
    this.inventoryService.getConsumables().subscribe({
      next: data => {
        this.consumables.set(data);
      },
      error: () => {
        this.toast.showError('Failed to load consumables');
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(ConsumableFormDialogComponent, {
      width: '480px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }
      this.inventoryService.createConsumable(result).subscribe({
        next: () => {
          this.toast.showSuccess('Consumable created successfully');
          this.loadConsumables();
        },
        error: err => {
          const detail = err?.error?.detail || err?.message || 'Failed to create consumable';
          this.toast.showError(`${detail}`);
        }
      });
    });
  }
}
