import { Component, Inject, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { UserDto, UserService } from '../../users/user.service';

export interface CheckoutDialogData {
  assetId: string;
  assetName: string;
}

@Component({
  selector: 'app-asset-checkout-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  templateUrl: './asset-checkout-dialog.component.html'
})
export class AssetCheckoutDialogComponent implements OnInit {
  private userService = inject(UserService);
  form: FormGroup;
  users = signal<UserDto[]>([]);

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<AssetCheckoutDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CheckoutDialogData
  ) {
    this.form = this.fb.group({
      userId: [null as string | null, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.userService.getUsers(1, 100).subscribe({
      next: (res) => this.users.set((res.items || []).filter(u => u.isActive)),
      error: () => this.users.set([])
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close({ userId: this.form.value.userId });
    }
  }
}
