import { Component, Inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';

export interface CheckoutDialogData {
  assetId: string;
  assetName: string;
}

@Component({
  selector: 'app-asset-checkout-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './asset-checkout-dialog.component.html'
})
export class AssetCheckoutDialogComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<AssetCheckoutDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CheckoutDialogData
  ) {
    this.form = this.fb.group({
      userId: ['', [Validators.required]] // Simple input for now since there's no Users API
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close({
        userId: this.form.value.userId
      });
    }
  }
}
