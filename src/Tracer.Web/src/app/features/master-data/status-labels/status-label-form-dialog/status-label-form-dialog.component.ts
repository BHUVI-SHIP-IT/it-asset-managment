import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { StatusLabel } from '../status-label.service';

export interface StatusLabelDialogData {
  statusLabel?: StatusLabel;
}

@Component({
  selector: 'app-status-label-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule
  ],
  templateUrl: './status-label-form-dialog.component.html'
})
export class StatusLabelFormDialogComponent {
  form: FormGroup;
  isEditMode: boolean;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<StatusLabelFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: StatusLabelDialogData
  ) {
    this.isEditMode = !!data.statusLabel;

    this.form = this.fb.group({
      name: [data.statusLabel?.name || '', [Validators.required, Validators.maxLength(50)]],
      isDeployable: [data.statusLabel?.isDeployable ?? false],
      isPending: [data.statusLabel?.isPending ?? false],
      isArchived: [data.statusLabel?.isArchived ?? false]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}
