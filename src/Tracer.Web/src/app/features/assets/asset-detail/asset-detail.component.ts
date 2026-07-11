import { Permissions } from '../../../core/auth/permissions';
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
  readonly permissions = Permissions;

  private route = inject(ActivatedRoute);
  private assetService = inject(AssetService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  asset = signal<AssetDetailDto | null>(null);
  history = signal<AssetHistoryDto[]>([]);
  loading = signal<boolean>(true);
  historyLoading = signal<boolean>(false);
  historyError = signal<string | null>(null);

  displayedHistoryColumns: string[] = ['validFrom', 'validTo', 'name', 'status', 'assignedUserId'];

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
        this.loading.set(false);
        this.loadHistory(id);
      },
      error: () => {
        this.snackBar.open('Error loading asset', 'Close', { duration: 5000 });
        this.loading.set(false);
      }
    });
  }

  loadHistory(id: string): void {
    this.historyLoading.set(true);
    this.historyError.set(null);
    this.assetService.getAssetHistory(id).subscribe({
      next: (data) => {
        this.history.set(Array.isArray(data) ? data : []);
        this.historyLoading.set(false);
      },
      error: (err) => {
        // Asset missing → 404; treat other failures as soft errors so the detail page still works.
        this.history.set([]);
        this.historyError.set(err?.error?.detail || 'Could not load asset history.');
        this.historyLoading.set(false);
      }
    });
  }

  /** Matches domain: Deployed + AssignedUserId (Asset.Checkin invariant). */
  isCheckedOut(asset: AssetDetailDto | null = this.asset()): boolean {
    if (!asset) return false;
    return asset.status === 'Deployed' && !!asset.assignedUserId;
  }

  /** Matches domain: Deployable and unassigned (Asset.Checkout invariant). */
  canCheckOut(asset: AssetDetailDto | null = this.asset()): boolean {
    if (!asset) return false;
    return asset.status === 'Deployable' && !asset.assignedUserId;
  }

  openCheckoutDialog(): void {
    const currentAsset = this.asset();
    if (!currentAsset) return;

    if (!this.canCheckOut(currentAsset)) {
      this.snackBar.open(
        this.isCheckedOut(currentAsset)
          ? 'This asset is already checked out. Check it in first.'
          : 'This asset cannot be checked out in its current status.',
        'Close',
        { duration: 5000 }
      );
      return;
    }

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
            this.snackBar.open(`Error: ${this.apiErrorDetail(err) || 'Failed to checkout asset'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  openCheckinDialog(): void {
    const currentAsset = this.asset();
    if (!currentAsset) return;

    if (!this.isCheckedOut(currentAsset)) {
      this.snackBar.open('This asset is not currently assigned.', 'Close', { duration: 5000 });
      return;
    }

    const dialogRef = this.dialog.open(AssetCheckinDialogComponent, {
      width: '400px',
      data: { assetId: currentAsset.id, assetName: currentAsset.name }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        // Re-read in case the signal changed while the dialog was open.
        const latest = this.asset();
        if (!latest || !this.isCheckedOut(latest)) {
          this.snackBar.open('This asset is not currently assigned.', 'Close', { duration: 5000 });
          return;
        }

        this.assetService.checkinAsset(latest.id, { assetId: latest.id }).subscribe({
          next: () => {
            this.snackBar.open('Asset checked in successfully', 'Close', { duration: 3000 });
            this.loadAsset(latest.id);
          },
          error: (err) => {
            this.snackBar.open(`Error: ${this.apiErrorDetail(err) || 'Failed to checkin asset'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  /** Prefer ProblemDetails.detail (e.g. 422 business-rule messages) over HttpErrorResponse.message. */
  private apiErrorDetail(err: { error?: { detail?: string; title?: string }; message?: string }): string | undefined {
    return err?.error?.detail || err?.error?.title || err?.message;
  }
}
