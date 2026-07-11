import { Permissions } from '../../../core/auth/permissions';
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

import { BaseTableComponent, PaginatedResult } from '../../../shared/components/base-table/base-table.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { AssetDto, AssetService } from '../asset.service';

@Component({
  selector: 'app-asset-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSnackBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    HasPermissionDirective
  ],
  templateUrl: './asset-list.component.html',
  styleUrls: ['./asset-list.component.scss']
})
export class AssetListComponent extends BaseTableComponent<AssetDto> implements OnInit {
  readonly permissions = Permissions;

  private assetService = inject(AssetService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private fb = inject(FormBuilder);
  displayedColumns = ['assetTag', 'name', 'status', 'purchaseCost', 'actions'];

  filterForm: FormGroup = this.fb.group({
    searchTerm: [''],
    status: [''],
    statusLabelId: [''],
    locationId: ['']
  });

  statusOptions = ['Deployable', 'Pending', 'Undeployable', 'Archived', 'Deployed'];
  statusLabels = signal<any[]>([]);
  locations = signal<any[]>([]);
  
  ngOnInit(): void {
    // Fetch lookups for filters
    this.http.get<any>('/api/v1/status-labels?pageSize=100').subscribe({
      next: res => this.statusLabels.set(Array.isArray(res) ? res : res.items || []),
      error: () => this.statusLabels.set([])
    });
    this.http.get<any>('/api/v1/locations?pageSize=100').subscribe({
      next: res => this.locations.set(Array.isArray(res) ? res : res.items || []),
      error: () => this.locations.set([])
    });

    // Listen for filter changes
    this.filterForm.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      this.pageIndex.set(0); // Reset to first page
      this.loadData();
    });

    this.loadData();
  }

  protected override fetchData(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<AssetDto>> {
    const filters = this.filterForm.value;
    return this.assetService.getAssets(
      page, 
      size, 
      sort, 
      order, 
      filters.searchTerm || undefined, 
      filters.status || undefined, 
      filters.statusLabelId ? parseInt(filters.statusLabelId, 10) : undefined, 
      filters.locationId || undefined
    );
  }

  deleteAsset(asset: AssetDto): void {
    if (confirm(`Are you sure you want to delete asset '${asset.assetTag}'?`)) {
      this.assetService.deleteAsset(asset.id).subscribe({
        next: () => {
          this.snackBar.open('Asset deleted successfully', 'Close', { duration: 3000 });
          this.loadData();
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to delete asset'}`, 'Close', { duration: 5000 });
        }
      });
    }
  }
}
