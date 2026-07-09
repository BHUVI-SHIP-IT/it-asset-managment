import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Company } from '../company.service';

export interface CompanyDialogData {
  company?: Company;
}

@Component({
  selector: 'app-company-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './company-form-dialog.component.html'
})
export class CompanyFormDialogComponent {
  form: FormGroup;
  isEditMode: boolean;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<CompanyFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CompanyDialogData
  ) {
    this.isEditMode = !!data.company;
    
    this.form = this.fb.group({
      name: [data.company?.name || '', [Validators.required, Validators.maxLength(100)]]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}
