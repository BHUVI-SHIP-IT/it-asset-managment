import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpClient } from '@angular/common/http';
import { AssetService } from '../asset.service';

@Component({
  selector: 'app-asset-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './asset-form.component.html',
  styleUrls: ['./asset-form.component.scss']
})
export class AssetFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private assetService = inject(AssetService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private http = inject(HttpClient);

  form!: FormGroup;
  isEditMode = false;
  assetId: string | null = null;
  loading = signal(false);
  submitting = signal(false);

  // Lookup data
  assetModels = signal<any[]>([]);
  statusLabels = signal<any[]>([]);
  locations = signal<any[]>([]);

  ngOnInit(): void {
    this.initForm();
    this.loadLookups();

    this.assetId = this.route.snapshot.paramMap.get('id');
    if (this.assetId) {
      this.isEditMode = true;
      this.loadAsset(this.assetId);
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      assetTag: ['', [Validators.required]],
      name: ['', [Validators.required]],
      assetModelId: [''],
      statusLabelId: ['', [Validators.required]],
      purchaseCost: [0, [Validators.required, Validators.min(0)]],
      locationId: [''],
      serialNumber: [''],
      purchaseDate: [''],
      notes: ['']
    });

    if (this.isEditMode) {
      this.form.get('assetTag')?.disable(); // AssetTag is create-only in this design
    }
  }

  private loadLookups(): void {
    // Quick fetching of lookups using HTTP client to avoid circular/missing service issues for now
    this.http.get<any>('/api/v1/asset-models?pageSize=100').subscribe(res => this.assetModels.set(Array.isArray(res) ? res : res.items || []));
    this.http.get<any>('/api/v1/status-labels?pageSize=100').subscribe(res => this.statusLabels.set(Array.isArray(res) ? res : res.items || []));
    this.http.get<any>('/api/v1/locations?pageSize=100').subscribe(res => this.locations.set(Array.isArray(res) ? res : res.items || []));
  }

  private loadAsset(id: string): void {
    this.loading.set(true);
    this.assetService.getAsset(id).subscribe({
      next: (asset) => {
        this.form.patchValue({
          assetTag: asset.assetTag,
          name: asset.name,
          assetModelId: asset.assetModelId,
          statusLabelId: asset.statusLabelId,
          purchaseCost: asset.purchaseCost,
          locationId: asset.locationId,
          serialNumber: asset.serialNumber,
          purchaseDate: asset.purchaseDate,
          notes: asset.notes
        });
        
        // Disable asset tag after patch in edit mode
        this.form.get('assetTag')?.disable();
        
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Error loading asset', 'Close', { duration: 3000 });
        this.loading.set(false);
        this.router.navigate(['/assets']);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.submitting.set(true);
    const formValue = this.form.getRawValue(); // gets disabled fields too

    if (this.isEditMode && this.assetId) {
      this.assetService.updateAsset(this.assetId, { ...formValue, id: this.assetId }).subscribe({
        next: () => {
          this.snackBar.open('Asset updated successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/assets', this.assetId]);
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to update asset'}`, 'Close', { duration: 5000 });
          this.submitting.set(false);
        }
      });
    } else {
      this.assetService.createAsset(formValue).subscribe({
        next: (newId) => {
          this.snackBar.open('Asset created successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/assets', newId]);
        },
        error: (err) => {
          this.snackBar.open(`Error: ${err.message || 'Failed to create asset'}`, 'Close', { duration: 5000 });
          this.submitting.set(false);
        }
      });
    }
  }
}
