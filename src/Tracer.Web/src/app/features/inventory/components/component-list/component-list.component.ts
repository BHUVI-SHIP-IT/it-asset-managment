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
import { MatTooltipModule } from '@angular/material/tooltip';
import { Observable } from 'rxjs';

import { BaseTableComponent, PaginatedResult } from '../../../../shared/components/base-table/base-table.component';
import { ConfirmDialogService } from '../../../../shared/components/confirm-dialog/confirm-dialog.service';
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
    MatTooltipModule,
    HasPermissionDirective
  ],
  templateUrl: './component-list.component.html',
  styleUrls: ['./component-list.component.scss']
})
export class ComponentListComponent extends BaseTableComponent<ComponentItem> implements OnInit {
  readonly permissions = Permissions;

  private componentService = inject(ComponentService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

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
            this.toast.showSuccess('Component created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create component'}`);
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
            this.toast.showSuccess('Component updated successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update component'}`);
          }
        });
      }
    });
  }

  deleteComponentItem(component: ComponentItem): void {
    this.confirmDialog
      .open({
        title: 'Delete component',
        message: `Are you sure you want to delete the component '${component.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.componentService.deleteComponent(component.id).subscribe({
          next: () => {
            this.toast.showSuccess('Component deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete component'}`);
          }
        });
      });
  }
}
