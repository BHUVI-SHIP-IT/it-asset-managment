import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CommonModule } from '@angular/common';
import { CustomFieldDto } from '../custom-field.service';

@Component({
  selector: 'app-custom-field-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule
  ],
  templateUrl: './custom-field-dialog.component.html'
})
export class CustomFieldDialogComponent implements OnInit {
  form: FormGroup;
  isEditMode: boolean;

  fieldTypes = [
    { value: 'text', label: 'Text' },
    { value: 'number', label: 'Number' },
    { value: 'date', label: 'Date' },
    { value: 'boolean', label: 'Boolean' },
    { value: 'dropdown', label: 'Dropdown' }
  ];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<CustomFieldDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CustomFieldDto | null
  ) {
    this.isEditMode = !!data;

    this.form = this.fb.group({
      name: ['', [Validators.required]],
      fieldType: ['text', [Validators.required]],
      isRequired: [false],
      options: ['']
    });
  }

  ngOnInit(): void {
    if (this.data) {
      this.form.patchValue({
        name: this.data.name,
        fieldType: this.data.fieldType,
        isRequired: this.data.isRequired,
        options: this.data.options || ''
      });
    }

    // Toggle options validation based on field type
    this.form.get('fieldType')?.valueChanges.subscribe(val => {
      const optionsCtrl = this.form.get('options');
      if (val === 'dropdown') {
        optionsCtrl?.setValidators([Validators.required]);
      } else {
        optionsCtrl?.clearValidators();
      }
      optionsCtrl?.updateValueAndValidity();
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close({
        ...this.data,
        ...this.form.value
      });
    }
  }
}
