import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ComponentItem } from '../component.service';

@Component({
  selector: 'app-component-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './component-form-dialog.component.html',
  styleUrls: ['./component-form-dialog.component.scss']
})
export class ComponentFormDialogComponent implements OnInit {
  form: FormGroup;
  isEditMode: boolean;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ComponentFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { component?: ComponentItem }
  ) {
    this.isEditMode = !!data.component;
    this.form = this.fb.group({
      name: [data.component?.name || '', [Validators.required]],
      totalQuantity: [data.component?.totalQuantity || 0, [Validators.required, Validators.min(0)]],
      purchaseCost: [data.component?.purchaseCost || 0, [Validators.required, Validators.min(0)]]
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
