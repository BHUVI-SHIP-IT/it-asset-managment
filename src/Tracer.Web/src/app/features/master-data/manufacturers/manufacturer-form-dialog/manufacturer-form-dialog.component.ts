import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Manufacturer } from '../manufacturer.service';

export interface ManufacturerDialogData {
  manufacturer?: Manufacturer;
}

@Component({
  selector: 'app-manufacturer-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './manufacturer-form-dialog.component.html'
})
export class ManufacturerFormDialogComponent {
  form: FormGroup;
  isEditMode: boolean;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ManufacturerFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ManufacturerDialogData
  ) {
    this.isEditMode = !!data.manufacturer;
    
    this.form = this.fb.group({
      name: [data.manufacturer?.name || '', [Validators.required, Validators.maxLength(100)]],
      url: [data.manufacturer?.url || '', [Validators.maxLength(250)]],
      supportUrl: [data.manufacturer?.supportUrl || '', [Validators.maxLength(250)]],
      supportPhone: [data.manufacturer?.supportPhone || '', [Validators.maxLength(50)]],
      supportEmail: [data.manufacturer?.supportEmail || '', [Validators.maxLength(100), Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}
