import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { LicenseDto } from '../../../../core/services/inventory';

@Component({
  selector: 'app-license-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  template: `
    <h2 mat-dialog-title>Create License</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="form-container">
        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name" required>
          <mat-error *ngIf="form.get('name')?.hasError('required')">Name is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Total Seats</mat-label>
          <input matInput type="number" formControlName="totalSeats" required min="1">
          <mat-error *ngIf="form.get('totalSeats')?.hasError('min')">At least 1 seat</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Purchase Cost</mat-label>
          <input matInput type="number" formControlName="purchaseCost" required min="0">
          <mat-error *ngIf="form.get('purchaseCost')?.hasError('min')">Cannot be negative</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Expiration Date</mat-label>
          <input matInput type="date" formControlName="expirationDate">
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Notes</mat-label>
          <textarea matInput formControlName="notes" rows="2"></textarea>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" [disabled]="form.invalid" (click)="onSubmit()">Create</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .form-container { display: flex; flex-direction: column; gap: 8px; min-width: 360px; padding-top: 8px; }
    .full-width { width: 100%; }
  `]
})
export class LicenseFormDialogComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<LicenseFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { license?: LicenseDto }
  ) {
    const exp = data.license?.expirationDate
      ? data.license.expirationDate.substring(0, 10)
      : '';
    this.form = this.fb.group({
      name: [data.license?.name || '', Validators.required],
      totalSeats: [data.license?.totalSeats ?? 1, [Validators.required, Validators.min(1)]],
      purchaseCost: [data.license?.purchaseCost ?? 0, [Validators.required, Validators.min(0)]],
      expirationDate: [exp],
      notes: [data.license?.notes || '']
    });
  }

  onSubmit(): void {
    if (!this.form.valid) {
      return;
    }
    const value = this.form.value;
    this.dialogRef.close({
      name: value.name,
      totalSeats: value.totalSeats,
      purchaseCost: value.purchaseCost,
      manufacturerId: null,
      expirationDate: value.expirationDate ? new Date(value.expirationDate).toISOString() : null,
      notes: value.notes || null
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
