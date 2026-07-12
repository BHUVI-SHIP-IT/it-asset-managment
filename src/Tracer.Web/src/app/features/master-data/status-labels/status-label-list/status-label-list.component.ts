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
import { StatusLabel, StatusLabelService } from '../status-label.service';
import { StatusLabelFormDialogComponent } from '../status-label-form-dialog/status-label-form-dialog.component';

@Component({
  selector: 'app-status-label-list',
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
  templateUrl: './status-label-list.component.html',
  styleUrls: ['./status-label-list.component.scss']
})
export class StatusLabelListComponent extends BaseTableComponent<StatusLabel> implements OnInit {
  readonly permissions = Permissions;

  private statusLabelService = inject(StatusLabelService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  displayedColumns = ['name', 'isDeployable', 'isPending', 'isArchived', 'actions'];

  ngOnInit(): void {
    // Initial load
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<StatusLabel>> {
    return this.statusLabelService.getStatusLabels(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(StatusLabelFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.statusLabelService.createStatusLabel(result).subscribe({
          next: () => {
            this.snackBar.open('Status label created successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to create status label'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  openEditDialog(statusLabel: StatusLabel): void {
    const dialogRef = this.dialog.open(StatusLabelFormDialogComponent, {
      width: '500px',
      data: { statusLabel }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.statusLabelService.updateStatusLabel(statusLabel.id, { ...result, id: statusLabel.id }).subscribe({
          next: () => {
            this.snackBar.open('Status label updated successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to update status label'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteStatusLabel(statusLabel: StatusLabel): void {
    if (confirm(`Are you sure you want to delete the status label '${statusLabel.name}'?`)) {
      this.statusLabelService.deleteStatusLabel(statusLabel.id).subscribe({
        next: () => {
          this.snackBar.open('Status label deleted successfully', 'Close', { duration: 3000 });
          this.loadData();
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to delete status label'}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
