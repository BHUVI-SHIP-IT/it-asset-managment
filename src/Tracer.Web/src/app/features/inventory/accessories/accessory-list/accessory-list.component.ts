import { Permissions } from '../../../../core/auth/permissions';
import { ToastService } from '../../../../core/ui/toast.service';
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Observable } from 'rxjs';

import { BaseTableComponent, PaginatedResult } from '../../../../shared/components/base-table/base-table.component';
import { ConfirmDialogService } from '../../../../shared/components/confirm-dialog/confirm-dialog.service';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { Accessory, AccessoryService } from '../accessory.service';
import { AccessoryFormDialogComponent } from '../accessory-form-dialog/accessory-form-dialog.component';

@Component({
  selector: 'app-accessory-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatTooltipModule,
    HasPermissionDirective
  ],
  templateUrl: './accessory-list.component.html',
  styleUrls: ['./accessory-list.component.scss']
})
export class AccessoryListComponent extends BaseTableComponent<Accessory> implements OnInit {
  readonly permissions = Permissions;

  private accessoryService = inject(AccessoryService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

  displayedColumns = ['name', 'totalQuantity', 'purchaseCost', 'actions'];

  ngOnInit(): void {
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<Accessory>> {
    return this.accessoryService.getAccessories(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(AccessoryFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.accessoryService.createAccessory(result).subscribe({
          next: () => {
            this.toast.showSuccess('Accessory created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create accessory'}`);
          }
        });
      }
    });
  }

  openEditDialog(accessory: Accessory): void {
    const dialogRef = this.dialog.open(AccessoryFormDialogComponent, {
      width: '500px',
      data: { accessory }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.accessoryService.updateAccessory(accessory.id, result).subscribe({
          next: () => {
            this.toast.showSuccess('Accessory updated successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update accessory'}`);
          }
        });
      }
    });
  }

  deleteAccessoryItem(accessory: Accessory): void {
    this.confirmDialog
      .open({
        title: 'Delete accessory',
        message: `Are you sure you want to delete the accessory '${accessory.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.accessoryService.deleteAccessory(accessory.id).subscribe({
          next: () => {
            this.toast.showSuccess('Accessory deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete accessory'}`);
          }
        });
      });
  }
}
