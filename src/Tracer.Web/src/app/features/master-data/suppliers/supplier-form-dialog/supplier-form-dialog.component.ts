import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Supplier } from '../supplier.service';

export interface SupplierDialogData {
  supplier?: Supplier;
}

@Component({
  selector: 'app-supplier-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './supplier-form-dialog.component.html'
})
export class SupplierFormDialogComponent {
  form: FormGroup;
  isEditMode: boolean;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<SupplierFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SupplierDialogData
  ) {
    this.isEditMode = !!data.supplier;
    
    this.form = this.fb.group({
      name: [data.supplier?.name || '', [Validators.required, Validators.maxLength(100)]],
      contactName: [data.supplier?.contactName || '', [Validators.maxLength(100)]],
      phone: [data.supplier?.phone || '', [Validators.maxLength(50)]],
      email: [data.supplier?.email || '', [Validators.maxLength(100), Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}
