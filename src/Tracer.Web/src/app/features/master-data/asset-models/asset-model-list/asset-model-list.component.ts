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
import { AssetModel, AssetModelService } from '../asset-model.service';
import { AssetModelFormDialogComponent } from '../asset-model-form-dialog/asset-model-form-dialog.component';

@Component({
  selector: 'app-asset-model-list',
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
  templateUrl: './asset-model-list.component.html',
  styleUrls: ['./asset-model-list.component.scss']
})
export class AssetModelListComponent extends BaseTableComponent<AssetModel> implements OnInit {
  readonly permissions = Permissions;

  private assetModelService = inject(AssetModelService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

  displayedColumns = ['name', 'actions'];

  ngOnInit(): void {
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<AssetModel>> {
    return this.assetModelService.getAssetModels(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(AssetModelFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.assetModelService.createAssetModel(result).subscribe({
          next: () => {
            this.toast.showSuccess('Asset Model created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create asset model'}`);
          }
        });
      }
    });
  }

  openEditDialog(assetModel: AssetModel): void {
    const dialogRef = this.dialog.open(AssetModelFormDialogComponent, {
      width: '500px',
      data: { assetModel }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.assetModelService.updateAssetModel(assetModel.id, { ...result, id: assetModel.id }).subscribe({
          next: () => {
            this.toast.showSuccess('Asset Model updated successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update asset model'}`);
          }
        });
      }
    });
  }

  deleteAssetModel(assetModel: AssetModel): void {
    this.confirmDialog
      .open({
        title: 'Delete asset model',
        message: `Are you sure you want to delete the asset model '${assetModel.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.assetModelService.deleteAssetModel(assetModel.id).subscribe({
          next: () => {
            this.toast.showSuccess('Asset Model deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete asset model'}`);
          }
        });
      });
  }
}
