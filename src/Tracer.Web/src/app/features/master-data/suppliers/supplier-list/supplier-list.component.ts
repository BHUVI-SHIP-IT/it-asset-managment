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
import { Observable } from 'rxjs';

import { BaseTableComponent, PaginatedResult } from '../../../../shared/components/base-table/base-table.component';
import { ConfirmDialogService } from '../../../../shared/components/confirm-dialog/confirm-dialog.service';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { Supplier, SupplierService } from '../supplier.service';
import { SupplierFormDialogComponent } from '../supplier-form-dialog/supplier-form-dialog.component';

@Component({
  selector: 'app-supplier-list',
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
    HasPermissionDirective
  ],
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss']
})
export class SupplierListComponent extends BaseTableComponent<Supplier> implements OnInit {
  readonly permissions = Permissions;

  private supplierService = inject(SupplierService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

  displayedColumns = ['name', 'actions'];

  ngOnInit(): void {
    // Initial load
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<Supplier>> {
    return this.supplierService.getSuppliers(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(SupplierFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.supplierService.createSupplier(result).subscribe({
          next: () => {
            this.toast.showSuccess('Supplier created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create supplier'}`);
          }
        });
      }
    });
  }

  openEditDialog(supplier: Supplier): void {
    const dialogRef = this.dialog.open(SupplierFormDialogComponent, {
      width: '500px',
      data: { supplier }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.supplierService.updateSupplier(supplier.id, { ...result, id: supplier.id }).subscribe({
          next: () => {
            this.toast.showSuccess('Supplier updated successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update supplier'}`);
          }
        });
      }
    });
  }

  deleteSupplier(supplier: Supplier): void {
    this.confirmDialog
      .open({
        title: 'Delete supplier',
        message: `Are you sure you want to delete the supplier '${supplier.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.supplierService.deleteSupplier(supplier.id).subscribe({
          next: () => {
            this.toast.showSuccess('Supplier deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete supplier'}`);
          }
        });
      });
  }
}
