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
    MatSnackBarModule,
    HasPermissionDirective
  ],
  templateUrl: './asset-model-list.component.html',
  styleUrls: ['./asset-model-list.component.scss']
})
export class AssetModelListComponent extends BaseTableComponent<AssetModel> implements OnInit {
  private assetModelService = inject(AssetModelService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

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
            this.snackBar.open('Asset Model created successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to create asset model'}`, 'Close', { duration: 5000 });
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
        this.assetModelService.updateAssetModel(assetModel.id, result).subscribe({
          next: () => {
            this.snackBar.open('Asset Model updated successfully', 'Close', { duration: 3000 });
            this.loadData();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to update asset model'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteAssetModel(assetModel: AssetModel): void {
    if (confirm(`Are you sure you want to delete the asset model '${assetModel.name}'?`)) {
      this.assetModelService.deleteAssetModel(assetModel.id).subscribe({
        next: () => {
          this.snackBar.open('Asset Model deleted successfully', 'Close', { duration: 3000 });
          this.loadData();
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to delete asset model'}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
