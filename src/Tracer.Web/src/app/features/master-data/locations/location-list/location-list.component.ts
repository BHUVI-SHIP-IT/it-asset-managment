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
import { Location, LocationService } from '../location.service';
import { LocationFormDialogComponent } from '../location-form-dialog/location-form-dialog.component';

@Component({
  selector: 'app-location-list',
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
  templateUrl: './location-list.component.html',
  styleUrls: ['./location-list.component.scss']
})
export class LocationListComponent extends BaseTableComponent<Location> implements OnInit {
  readonly permissions = Permissions;

  private locationService = inject(LocationService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

  displayedColumns = ['name', 'actions'];

  ngOnInit(): void {
    // Initial load
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<Location>> {
    return this.locationService.getLocations(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(LocationFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.locationService.createLocation(result).subscribe({
          next: () => {
            this.toast.showSuccess('Location created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create location'}`);
          }
        });
      }
    });
  }

  openEditDialog(location: Location): void {
    const dialogRef = this.dialog.open(LocationFormDialogComponent, {
      width: '500px',
      data: { location }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.locationService.updateLocation(location.id, { ...result, id: location.id }).subscribe({
          next: () => {
            this.toast.showSuccess('Location updated successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update location'}`);
          }
        });
      }
    });
  }

  deleteLocation(location: Location): void {
    this.confirmDialog
      .open({
        title: 'Delete location',
        message: `Are you sure you want to delete the location '${location.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.locationService.deleteLocation(location.id).subscribe({
          next: () => {
            this.toast.showSuccess('Location deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete location'}`);
          }
        });
      });
  }
}
