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
import { Manufacturer, ManufacturerService } from '../manufacturer.service';
import { ManufacturerFormDialogComponent } from '../manufacturer-form-dialog/manufacturer-form-dialog.component';

@Component({
  selector: 'app-manufacturer-list',
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
  templateUrl: './manufacturer-list.component.html',
  styleUrls: ['./manufacturer-list.component.scss']
})
export class ManufacturerListComponent extends BaseTableComponent<Manufacturer> implements OnInit {
  readonly permissions = Permissions;

  private manufacturerService = inject(ManufacturerService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  displayedColumns = ['name', 'url', 'supportEmail', 'actions'];

  ngOnInit(): void {
    // Initial load
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<Manufacturer>> {
    return this.manufacturerService.getManufacturers(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(ManufacturerFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.manufacturerService.createManufacturer(result).subscribe({
          next: () => {
            this.snackBar.open('Manufacturer created successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to create manufacturer'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  openEditDialog(manufacturer: Manufacturer): void {
    const dialogRef = this.dialog.open(ManufacturerFormDialogComponent, {
      width: '500px',
      data: { manufacturer }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.manufacturerService.updateManufacturer(manufacturer.id, result).subscribe({
          next: () => {
            this.snackBar.open('Manufacturer updated successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to update manufacturer'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteManufacturer(manufacturer: Manufacturer): void {
    if (confirm(`Are you sure you want to delete the manufacturer '${manufacturer.name}'?`)) {
      this.manufacturerService.deleteManufacturer(manufacturer.id).subscribe({
        next: () => {
          this.snackBar.open('Manufacturer deleted successfully', 'Close', { duration: 3000 });
          this.loadData();
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to delete manufacturer'}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
