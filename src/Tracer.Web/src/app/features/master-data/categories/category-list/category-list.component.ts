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
import { Category, CategoryService } from '../category.service';
import { CategoryFormDialogComponent } from '../category-form-dialog/category-form-dialog.component';

@Component({
  selector: 'app-category-list',
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
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.scss']
})
export class CategoryListComponent extends BaseTableComponent<Category> implements OnInit {
  readonly permissions = Permissions;

  private categoryService = inject(CategoryService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

  displayedColumns = ['name', 'actions'];

  ngOnInit(): void {
    // Initial load
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<Category>> {
    return this.categoryService.getCategories(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CategoryFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.categoryService.createCategory(result).subscribe({
          next: () => {
            this.toast.showSuccess('Category created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create category'}`);
          }
        });
      }
    });
  }

  openEditDialog(category: Category): void {
    const dialogRef = this.dialog.open(CategoryFormDialogComponent, {
      width: '500px',
      data: { category }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Route id must be in the body — UpdateCategoryCommand requires Id.
        this.categoryService.updateCategory(category.id, { ...result, id: category.id }).subscribe({
          next: () => {
            this.toast.showSuccess('Category updated successfully');
            this.loadData();
          },
          error: (err) => {
            const detail = err?.error?.detail || err?.error?.title || err?.message || 'Failed to update category';
            this.toast.showError(`${detail}`);
          }
        });
      }
    });
  }

  deleteCategory(category: Category): void {
    this.confirmDialog
      .open({
        title: 'Delete category',
        message: `Are you sure you want to delete the category '${category.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.categoryService.deleteCategory(category.id).subscribe({
          next: () => {
            this.toast.showSuccess('Category deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete category'}`);
          }
        });
      });
  }
}
