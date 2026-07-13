import { Permissions } from '../../../core/auth/permissions';
import { ToastService } from '../../../core/ui/toast.service';
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CustomFieldService, CustomFieldDto } from './custom-field.service';
import { CustomFieldDialogComponent } from './custom-field-dialog/custom-field-dialog.component';
import { ConfirmDialogService } from '../../../shared/components/confirm-dialog/confirm-dialog.service';
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
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

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
        this.toast.showError('Error loading custom fields');
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
            this.toast.showSuccess('Custom field created successfully');
            this.loadFields();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create field'}`);
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
            this.toast.showSuccess('Custom field updated successfully');
            this.loadFields();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update field'}`);
          }
        });
      }
    });
  }

  deleteField(field: CustomFieldDto): void {
    this.confirmDialog
      .open({
        title: 'Delete custom field',
        message: `Are you sure you want to delete ${field.name}?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.customFieldService.deleteField(field.id).subscribe({
          next: () => {
            this.toast.showSuccess('Custom field deleted successfully');
            this.loadFields();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete field'}`);
          }
        });
      });
  }
}
