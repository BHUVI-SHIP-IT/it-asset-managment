import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Accessory } from '../accessory.service';

@Component({
  selector: 'app-accessory-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './accessory-form-dialog.component.html',
  styleUrls: ['./accessory-form-dialog.component.scss']
})
export class AccessoryFormDialogComponent implements OnInit {
  form: FormGroup;
  isEditMode: boolean;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AccessoryFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { accessory?: Accessory }
  ) {
    this.isEditMode = !!data.accessory;
    this.form = this.fb.group({
      name: [data.accessory?.name || '', [Validators.required]],
      totalQuantity: [data.accessory?.totalQuantity || 0, [Validators.required, Validators.min(0)]],
      purchaseCost: [data.accessory?.purchaseCost || 0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
