import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { InventoryService, LicenseDto } from '../../../../core/services/inventory';

@Component({
  selector: 'app-license-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatDialogModule],
  template: `
    <div class="header">
      <h2>Licenses</h2>
      <button mat-raised-button color="primary">
        <mat-icon>add</mat-icon> Create License
      </button>
    </div>

    <table mat-table [dataSource]="licenses" class="mat-elevation-z8">
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

      <ng-container matColumnDef="actions">
        <th mat-header-cell *matHeaderCellDef> Actions </th>
        <td mat-cell *matCellDef="let element">
          <button mat-icon-button color="primary">
            <mat-icon>edit</mat-icon>
          </button>
        </td>
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
  private inventoryService = inject(InventoryService);
  private dialog = inject(MatDialog);

  licenses: LicenseDto[] = [];
  displayedColumns: string[] = ['name', 'seats', 'cost', 'expiration', 'actions'];

  ngOnInit() {
    this.loadLicenses();
  }

  loadLicenses() {
    this.inventoryService.getLicenses().subscribe(data => {
      this.licenses = data;
    });
  }
}
