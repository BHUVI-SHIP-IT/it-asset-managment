import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { AssetDetailDto, AssetHistoryDto, AssetService } from '../asset.service';
import { AssetCheckoutDialogComponent } from '../asset-checkout-dialog/asset-checkout-dialog.component';
import { AssetCheckinDialogComponent } from '../asset-checkin-dialog/asset-checkin-dialog.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-asset-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTableModule,
    HasPermissionDirective
  ],
  templateUrl: './asset-detail.component.html',
  styleUrls: ['./asset-detail.component.scss']
})
export class AssetDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private assetService = inject(AssetService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  asset = signal<AssetDetailDto | null>(null);
  history = signal<AssetHistoryDto[]>([]);
  loading = signal<boolean>(true);

  displayedHistoryColumns: string[] = ['validFrom', 'validTo', 'name', 'assignedUserId', 'statusLabelId'];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadAsset(id);
    }
  }

  loadAsset(id: string): void {
    this.loading.set(true);
    this.assetService.getAsset(id).subscribe({
      next: (data) => {
        this.asset.set(data);
        this.loadHistory(id);
      },
      error: (err) => {
        this.snackBar.open('Error loading asset', 'Close', { duration: 5000 });
        this.loading.set(false);
      }
    });
  }

  loadHistory(id: string): void {
    this.assetService.getAssetHistory(id).subscribe({
      next: (data) => {
        this.history.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.snackBar.open('Error loading asset history', 'Close', { duration: 5000 });
        this.loading.set(false);
      }
    });
  }

  openCheckoutDialog(): void {
    const currentAsset = this.asset();
    if (!currentAsset) return;

    const dialogRef = this.dialog.open(AssetCheckoutDialogComponent, {
      width: '400px',
      data: { assetId: currentAsset.id, assetName: currentAsset.name }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && result.userId) {
        this.assetService.checkoutAsset(currentAsset.id, { assetId: currentAsset.id, userId: result.userId }).subscribe({
          next: () => {
            this.snackBar.open('Asset checked out successfully', 'Close', { duration: 3000 });
            this.loadAsset(currentAsset.id);
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to checkout asset'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  openCheckinDialog(): void {
    const currentAsset = this.asset();
    if (!currentAsset) return;

    const dialogRef = this.dialog.open(AssetCheckinDialogComponent, {
      width: '400px',
      data: { assetId: currentAsset.id, assetName: currentAsset.name }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.assetService.checkinAsset(currentAsset.id, { assetId: currentAsset.id }).subscribe({
          next: () => {
            this.snackBar.open('Asset checked in successfully', 'Close', { duration: 3000 });
            this.loadAsset(currentAsset.id);
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to checkin asset'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }
}
