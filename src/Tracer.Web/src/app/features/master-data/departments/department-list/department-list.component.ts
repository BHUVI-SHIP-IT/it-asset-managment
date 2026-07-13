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
import { Department, DepartmentService } from '../department.service';
import { DepartmentFormDialogComponent } from '../department-form-dialog/department-form-dialog.component';

@Component({
  selector: 'app-department-list',
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
  templateUrl: './department-list.component.html',
  styleUrls: ['./department-list.component.scss']
})
export class DepartmentListComponent extends BaseTableComponent<Department> implements OnInit {
  readonly permissions = Permissions;

  private departmentService = inject(DepartmentService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

  displayedColumns = ['name', 'actions'];

  ngOnInit(): void {
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<Department>> {
    return this.departmentService.getDepartments(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(DepartmentFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.departmentService.createDepartment(result).subscribe({
          next: () => {
            this.toast.showSuccess('Department created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create department'}`);
          }
        });
      }
    });
  }

  openEditDialog(department: Department): void {
    const dialogRef = this.dialog.open(DepartmentFormDialogComponent, {
      width: '500px',
      data: { department }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.departmentService.updateDepartment(department.id, { ...result, id: department.id }).subscribe({
          next: () => {
            this.toast.showSuccess('Department updated successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update department'}`);
          }
        });
      }
    });
  }

  deleteDepartment(department: Department): void {
    this.confirmDialog
      .open({
        title: 'Delete department',
        message: `Are you sure you want to delete the department '${department.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.departmentService.deleteDepartment(department.id).subscribe({
          next: () => {
            this.toast.showSuccess('Department deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete department'}`);
          }
        });
      });
  }
}
