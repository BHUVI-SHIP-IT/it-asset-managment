import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

export interface CheckinDialogData {
  assetId: string;
  assetName: string;
}

@Component({
  selector: 'app-asset-checkin-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule
  ],
  template: `
    <h2 mat-dialog-title>Checkin {{ data.assetName }}</h2>
    <mat-dialog-content>
      <p>Are you sure you want to return this asset to inventory?</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary" (click)="onConfirm()">Confirm Checkin</button>
    </mat-dialog-actions>
  `
})
export class AssetCheckinDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<AssetCheckinDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CheckinDialogData
  ) {}

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}
