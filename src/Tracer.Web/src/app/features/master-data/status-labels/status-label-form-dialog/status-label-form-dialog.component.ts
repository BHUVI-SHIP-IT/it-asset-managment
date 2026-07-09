import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
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
    MatSelectModule
  ],
  templateUrl: './status-label-form-dialog.component.html'
})
export class StatusLabelFormDialogComponent {
  form: FormGroup;
  isEditMode: boolean;
  
  statusTypes = ['Deployable', 'Pending', 'Undeployable', 'Archived'];

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<StatusLabelFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: StatusLabelDialogData
  ) {
    this.isEditMode = !!data.statusLabel;
    
    this.form = this.fb.group({
      name: [data.statusLabel?.name || '', [Validators.required, Validators.maxLength(50)]],
      statusType: [data.statusLabel?.statusType || '', [Validators.required]],
      colorHex: [data.statusLabel?.colorHex || '#000000', [Validators.maxLength(7)]]
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}
