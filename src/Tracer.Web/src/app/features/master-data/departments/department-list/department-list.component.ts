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
    MatSnackBarModule,
    HasPermissionDirective
  ],
  templateUrl: './department-list.component.html',
  styleUrls: ['./department-list.component.scss']
})
export class DepartmentListComponent extends BaseTableComponent<Department> implements OnInit {
  readonly permissions = Permissions;

  private departmentService = inject(DepartmentService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

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
            this.snackBar.open('Department created successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to create department'}`, 'Close', { duration: 5000 });
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
            this.snackBar.open('Department updated successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to update department'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteDepartment(department: Department): void {
    if (confirm(`Are you sure you want to delete the department '${department.name}'?`)) {
      this.departmentService.deleteDepartment(department.id).subscribe({
        next: () => {
          this.snackBar.open('Department deleted successfully', 'Close', { duration: 3000 });
          this.loadData();
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to delete department'}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
