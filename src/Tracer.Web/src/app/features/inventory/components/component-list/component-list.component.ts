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
import { ComponentItem, ComponentService } from '../component.service';
import { ComponentFormDialogComponent } from '../component-form-dialog/component-form-dialog.component';

@Component({
  selector: 'app-component-list',
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
  templateUrl: './component-list.component.html',
  styleUrls: ['./component-list.component.scss']
})
export class ComponentListComponent extends BaseTableComponent<ComponentItem> implements OnInit {
  readonly permissions = Permissions;

  private componentService = inject(ComponentService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  displayedColumns = ['name', 'totalQuantity', 'purchaseCost', 'actions'];

  ngOnInit(): void {
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<ComponentItem>> {
    return this.componentService.getComponents(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(ComponentFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.componentService.createComponent(result).subscribe({
          next: () => {
            this.snackBar.open('Component created successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to create component'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  openEditDialog(component: ComponentItem): void {
    const dialogRef = this.dialog.open(ComponentFormDialogComponent, {
      width: '500px',
      data: { component }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.componentService.updateComponent(component.id, result).subscribe({
          next: () => {
            this.snackBar.open('Component updated successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to update component'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteComponentItem(component: ComponentItem): void {
    if (confirm(`Are you sure you want to delete the component '${component.name}'?`)) {
      this.componentService.deleteComponent(component.id).subscribe({
        next: () => {
          this.snackBar.open('Component deleted successfully', 'Close', { duration: 3000 });
          this.loadData();
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to delete component'}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
