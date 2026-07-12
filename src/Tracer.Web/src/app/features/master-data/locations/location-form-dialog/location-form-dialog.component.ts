import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Location } from '../location.service';

export interface LocationDialogData {
  location?: Location;
}

@Component({
  selector: 'app-location-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './location-form-dialog.component.html'
})
export class LocationFormDialogComponent {
  form: FormGroup;
  isEditMode: boolean;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<LocationFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: LocationDialogData
  ) {
    this.isEditMode = !!data.location;

    this.form = this.fb.group({
      name: [data.location?.name || '', [Validators.required, Validators.maxLength(100)]]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}
