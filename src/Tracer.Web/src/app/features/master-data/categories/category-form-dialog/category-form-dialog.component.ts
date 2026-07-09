import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Category } from '../category.service';

export interface CategoryDialogData {
  category?: Category;
}

@Component({
  selector: 'app-category-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './category-form-dialog.component.html'
})
export class CategoryFormDialogComponent {
  form: FormGroup;
  isEditMode: boolean;
  
  categoryTypes = ['Asset', 'Component', 'Consumable', 'License'];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<CategoryFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CategoryDialogData
  ) {
    this.isEditMode = !!data.category;
    
    this.form = this.fb.group({
      name: [data.category?.name || '', [Validators.required, Validators.maxLength(50)]],
      description: [data.category?.description || '', [Validators.maxLength(200)]],
      categoryType: [data.category?.categoryType || '', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}
