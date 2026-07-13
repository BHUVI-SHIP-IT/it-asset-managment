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
    HasPermissionDirective
  ],
  templateUrl: './status-label-list.component.html',
  styleUrls: ['./status-label-list.component.scss']
})
export class StatusLabelListComponent extends BaseTableComponent<StatusLabel> implements OnInit {
  readonly permissions = Permissions;

  private statusLabelService = inject(StatusLabelService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

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
            this.toast.showSuccess('Status label created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create status label'}`);
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
            this.toast.showSuccess('Status label updated successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update status label'}`);
          }
        });
      }
    });
  }

  deleteStatusLabel(statusLabel: StatusLabel): void {
    this.confirmDialog
      .open({
        title: 'Delete status label',
        message: `Are you sure you want to delete the status label '${statusLabel.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.statusLabelService.deleteStatusLabel(statusLabel.id).subscribe({
          next: () => {
            this.toast.showSuccess('Status label deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete status label'}`);
          }
        });
      });
  }
}
