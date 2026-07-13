import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Permissions } from '../../../../core/auth/permissions';
import { ToastService } from '../../../../core/ui/toast.service';
import { ConfirmDialogService } from '../../../../shared/components/confirm-dialog/confirm-dialog.service';
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
    MatTooltipModule,
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

      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef class="action-column"> Actions </th>
        <td mat-cell *matCellDef="let element" class="action-column">
          <button mat-icon-button color="primary" type="button"
                  *hasPermission="permissions.Consumables.Update"
                  (click)="openEditDialog(element)"
                  matTooltip="Edit" aria-label="Edit">
            <mat-icon>edit</mat-icon>
          </button>
          <button mat-icon-button color="warn" type="button"
                  *hasPermission="permissions.Consumables.Delete"
                  (click)="deleteConsumable(element)"
                  matTooltip="Delete" aria-label="Delete">
            <mat-icon>delete</mat-icon>
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
    .action-column {
      width: 120px;
      text-align: right;
      overflow: visible;
      text-overflow: clip;
      white-space: nowrap;
    }
  `]
})
export class ConsumableListComponent implements OnInit {
  readonly permissions = Permissions;

  private inventoryService = inject(InventoryService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

  consumables = signal<ConsumableDto[]>([]);
  displayedColumns: string[] = ['id', 'name', 'quantity', 'cost', 'actions'];

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

  openEditDialog(consumable: ConsumableDto): void {
    const dialogRef = this.dialog.open(ConsumableFormDialogComponent, {
      width: '480px',
      data: { consumable }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }
      this.inventoryService.updateConsumable(consumable.id, result).subscribe({
        next: () => {
          this.toast.showSuccess('Consumable updated successfully');
          this.loadConsumables();
        },
        error: err => {
          const detail = err?.error?.detail || err?.message || 'Failed to update consumable';
          this.toast.showError(`${detail}`);
        }
      });
    });
  }

  deleteConsumable(consumable: ConsumableDto): void {
    this.confirmDialog
      .open({
        title: 'Delete consumable',
        message: `Are you sure you want to delete the consumable '${consumable.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.inventoryService.deleteConsumable(consumable.id).subscribe({
          next: () => {
            this.toast.showSuccess('Consumable deleted successfully');
            this.loadConsumables();
          },
          error: err => {
            const detail = err?.error?.detail || err?.message || 'Failed to delete consumable';
            this.toast.showError(`${detail}`);
          }
        });
      });
  }
}
