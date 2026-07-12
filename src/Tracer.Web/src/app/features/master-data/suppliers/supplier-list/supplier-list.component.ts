import { Permissions } from '../../../../core/auth/permissions';
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';

import { BaseTableComponent, PaginatedResult } from '../../../../shared/components/base-table/base-table.component';
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
    MatSnackBarModule,
    HasPermissionDirective
  ],
  templateUrl: './supplier-list.component.html',
  styleUrls: ['./supplier-list.component.scss']
})
export class SupplierListComponent extends BaseTableComponent<Supplier> implements OnInit {
  readonly permissions = Permissions;

  private supplierService = inject(SupplierService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

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
            this.snackBar.open('Supplier created successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to create supplier'}`, 'Close', { duration: 5000 });
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
            this.snackBar.open('Supplier updated successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to update supplier'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteSupplier(supplier: Supplier): void {
    if (confirm(`Are you sure you want to delete the supplier '${supplier.name}'?`)) {
      this.supplierService.deleteSupplier(supplier.id).subscribe({
        next: () => {
          this.snackBar.open('Supplier deleted successfully', 'Close', { duration: 3000 });
          this.loadData();
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to delete supplier'}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
