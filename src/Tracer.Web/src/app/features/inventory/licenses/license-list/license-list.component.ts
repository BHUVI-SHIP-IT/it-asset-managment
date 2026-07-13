import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { Permissions } from '../../../../core/auth/permissions';
import { ToastService } from '../../../../core/ui/toast.service';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { InventoryService, LicenseDto } from '../../../../core/services/inventory';
import { LicenseFormDialogComponent } from '../license-form-dialog/license-form-dialog.component';

@Component({
  selector: 'app-license-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    HasPermissionDirective
  ],
  template: `
    <div class="header">
      <h2>Licenses</h2>
      <button mat-raised-button color="primary"
              *hasPermission="permissions.Licenses.Create"
              (click)="openCreateDialog()">
        <mat-icon>add</mat-icon> Create License
      </button>
    </div>

    <table mat-table [dataSource]="licenses()" class="mat-elevation-z8">
      <ng-container matColumnDef="name">
        <th mat-header-cell *matHeaderCellDef> Name </th>
        <td mat-cell *matCellDef="let element"> {{element.name}} </td>
      </ng-container>

      <ng-container matColumnDef="seats">
        <th mat-header-cell *matHeaderCellDef> Total Seats </th>
        <td mat-cell *matCellDef="let element"> {{element.totalSeats}} </td>
      </ng-container>

      <ng-container matColumnDef="cost">
        <th mat-header-cell *matHeaderCellDef> Purchase Cost </th>
        <td mat-cell *matCellDef="let element"> {{element.purchaseCost | currency}} </td>
      </ng-container>

      <ng-container matColumnDef="expiration">
        <th mat-header-cell *matHeaderCellDef> Expiration </th>
        <td mat-cell *matCellDef="let element"> {{element.expirationDate | date}} </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
    </table>
  `,
  styles: [`
    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }
    table {
      width: 100%;
    }
  `]
})
export class LicenseListComponent implements OnInit {
  readonly permissions = Permissions;

  private inventoryService = inject(InventoryService);
  private dialog = inject(MatDialog);
  private toast = inject(ToastService);

  licenses = signal<LicenseDto[]>([]);
  displayedColumns: string[] = ['name', 'seats', 'cost', 'expiration'];

  ngOnInit() {
    this.loadLicenses();
  }

  loadLicenses() {
    this.inventoryService.getLicenses().subscribe({
      next: data => {
        this.licenses.set(data);
      },
      error: () => {
        this.toast.showError('Failed to load licenses');
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(LicenseFormDialogComponent, {
      width: '480px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }
      this.inventoryService.createLicense(result).subscribe({
        next: () => {
          this.toast.showSuccess('License created successfully');
          this.loadLicenses();
        },
        error: err => {
          const detail = err?.error?.detail || err?.message || 'Failed to create license';
          this.toast.showError(`${detail}`);
        }
      });
    });
  }
}
