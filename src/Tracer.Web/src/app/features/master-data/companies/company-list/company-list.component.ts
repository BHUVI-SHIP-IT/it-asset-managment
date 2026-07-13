import { Permissions } from '../../../../core/auth/permissions';
import { ToastService } from '../../../../core/ui/toast.service';
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { Observable } from 'rxjs';

import { BaseTableComponent, PaginatedResult } from '../../../../shared/components/base-table/base-table.component';
import { ConfirmDialogService } from '../../../../shared/components/confirm-dialog/confirm-dialog.service';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { Company, CompanyService } from '../company.service';
import { CompanyFormDialogComponent } from '../company-form-dialog/company-form-dialog.component';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    HasPermissionDirective
  ],
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.scss']
})
export class CompanyListComponent extends BaseTableComponent<Company> implements OnInit {
  readonly permissions = Permissions;

  private companyService = inject(CompanyService);
  private dialog = inject(MatDialog);
  private confirmDialog = inject(ConfirmDialogService);
  private toast = inject(ToastService);

  displayedColumns = ['name', 'actions'];

  ngOnInit(): void {
    // Initial load
    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<Company>> {
    return this.companyService.getCompanies(page, size, sort, order);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CompanyFormDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.companyService.createCompany(result).subscribe({
          next: () => {
            this.toast.showSuccess('Company created successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to create company'}`);
          }
        });
      }
    });
  }

  openEditDialog(company: Company): void {
    const dialogRef = this.dialog.open(CompanyFormDialogComponent, {
      width: '500px',
      data: { company }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.companyService.updateCompany(company.id, result).subscribe({
          next: () => {
            this.toast.showSuccess('Company updated successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to update company'}`);
          }
        });
      }
    });
  }

  deleteCompany(company: Company): void {
    this.confirmDialog
      .open({
        title: 'Delete company',
        message: `Are you sure you want to delete the company '${company.name}'?`,
        confirmText: 'Delete'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.companyService.deleteCompany(company.id).subscribe({
          next: () => {
            this.toast.showSuccess('Company deleted successfully');
            this.loadData();
          },
          error: (err) => {
            this.toast.showError(`${err.message || 'Failed to delete company'}`);
          }
        });
      });
  }
}
