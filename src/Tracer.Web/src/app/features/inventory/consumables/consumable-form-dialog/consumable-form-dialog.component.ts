import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ConsumableDto } from '../../../../core/services/inventory';

@Component({
  selector: 'app-consumable-form-dialog',
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
    <h2 mat-dialog-title>Create Consumable</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="form-container">
        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name" required>
          <mat-error *ngIf="form.get('name')?.hasError('required')">Name is required</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Total Quantity</mat-label>
          <input matInput type="number" formControlName="totalQuantity" required min="0">
          <mat-error *ngIf="form.get('totalQuantity')?.hasError('min')">Cannot be negative</mat-error>
        </mat-form-field>

        <mat-form-field appearance="fill" class="full-width">
          <mat-label>Purchase Cost</mat-label>
          <input matInput type="number" formControlName="purchaseCost" required min="0">
          <mat-error *ngIf="form.get('purchaseCost')?.hasError('min')">Cannot be negative</mat-error>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" [disabled]="form.invalid" (click)="onSubmit()">Create</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .form-container { display: flex; flex-direction: column; gap: 8px; min-width: 320px; padding-top: 8px; }
    .full-width { width: 100%; }
  `]
})
export class ConsumableFormDialogComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ConsumableFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { consumable?: ConsumableDto }
  ) {
    this.form = this.fb.group({
      name: [data.consumable?.name || '', Validators.required],
      totalQuantity: [data.consumable?.totalQuantity ?? 0, [Validators.required, Validators.min(0)]],
      purchaseCost: [data.consumable?.purchaseCost ?? 0, [Validators.required, Validators.min(0)]]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
