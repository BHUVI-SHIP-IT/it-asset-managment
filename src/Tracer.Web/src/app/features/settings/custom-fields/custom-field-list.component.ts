import { Permissions } from '../../../core/auth/permissions';
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CustomFieldService, CustomFieldDto } from './custom-field.service';
import { CustomFieldDialogComponent } from './custom-field-dialog/custom-field-dialog.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-custom-field-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule,
    HasPermissionDirective,
    ],
  templateUrl: './custom-field-list.component.html',
  styleUrls: ['./custom-field-list.component.scss']
})
export class CustomFieldListComponent implements OnInit {
  readonly permissions = Permissions;

  private customFieldService = inject(CustomFieldService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  displayedColumns: string[] = ['name', 'fieldType', 'isRequired', 'options', 'actions'];
  dataSource = signal<CustomFieldDto[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.loadFields();
  }

  loadFields(): void {
    this.loading.set(true);
    this.customFieldService.getAllFields().subscribe({
      next: (fields) => {
        this.dataSource.set(fields);
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Error loading custom fields', 'Close', { duration: 3000 });
        this.loading.set(false);
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CustomFieldDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.customFieldService.createField(result).subscribe({
          next: () => {
            this.snackBar.open('Custom field created successfully', 'Close', { duration: 3000 });
            this.loadFields();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to create field'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  openEditDialog(field: CustomFieldDto): void {
    const dialogRef = this.dialog.open(CustomFieldDialogComponent, {
      width: '400px',
      data: field
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.customFieldService.updateField(field.id, result).subscribe({
          next: () => {
            this.snackBar.open('Custom field updated successfully', 'Close', { duration: 3000 });
            this.loadFields();
          },
          error: (err) => {
            this.snackBar.open(`Error: ${err.message || 'Failed to update field'}`, 'Close', { duration: 5000 });
          }
        });
      }
    });
  }

  deleteField(field: CustomFieldDto): void {
    if (confirm(`Are you sure you want to delete ${field.name}?`)) {
      this.customFieldService.deleteField(field.id).subscribe({
        next: () => {
          this.snackBar.open('Custom field deleted successfully', 'Close', { duration: 3000 });
          this.loadFields();
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to delete field'}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
