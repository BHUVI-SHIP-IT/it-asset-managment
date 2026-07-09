import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Department } from '../department.service';

@Component({
  selector: 'app-department-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './department-form-dialog.component.html',
  styles: [`
    mat-form-field { width: 100%; }
  `]
})
export class DepartmentFormDialogComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<DepartmentFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { department?: Department }
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    if (this.data.department) {
      this.isEditMode = true;
      this.form.patchValue({
        name: this.data.department.name
      });
    }
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    } else {
      this.form.markAllAsTouched();
    }
  }
}
