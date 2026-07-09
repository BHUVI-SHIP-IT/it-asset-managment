import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AssetModel } from '../asset-model.service';

@Component({
  selector: 'app-asset-model-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './asset-model-form-dialog.component.html',
  styles: [`
    mat-form-field { width: 100%; }
  `]
})
export class AssetModelFormDialogComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AssetModelFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { assetModel?: AssetModel }
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    if (this.data.assetModel) {
      this.isEditMode = true;
      this.form.patchValue({
        name: this.data.assetModel.name
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
