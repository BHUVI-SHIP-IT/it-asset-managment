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
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ToastService } from '../../../core/ui/toast.service';
import { AssetService, CreateAssetCommand, UpdateAssetCommand } from '../asset.service';

interface LookupItem {
  id: string | number;
  name: string;
}

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
  private toast = inject(ToastService);
  private http = inject(HttpClient);

  form!: FormGroup;
  isEditMode = false;
  assetId: string | null = null;
  loading = signal(false);
  submitting = signal(false);

  assetModels = signal<LookupItem[]>([]);
  statusLabels = signal<LookupItem[]>([]);
  locations = signal<LookupItem[]>([]);

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
      assetModelId: [null as string | null, [Validators.required]],
      statusLabelId: [null as number | null, [Validators.required]],
      purchaseCost: [0, [Validators.required, Validators.min(0)]],
      locationId: [null as string | null],
      serialNumber: [''],
      purchaseDate: [null as Date | null],
      notes: ['']
    });

    if (this.isEditMode) {
      this.form.get('assetTag')?.disable();
    }
  }

  private loadLookups(): void {
    const params = new HttpParams().set('pageNumber', '1').set('pageSize', '100');

    this.http.get<{ items?: LookupItem[] } | LookupItem[]>('/api/v1/asset-models', { params })
      .subscribe(res => this.assetModels.set(Array.isArray(res) ? res : res.items || []));
    this.http.get<{ items?: LookupItem[] } | LookupItem[]>('/api/v1/status-labels', { params })
      .subscribe(res => this.statusLabels.set(Array.isArray(res) ? res : res.items || []));
    this.http.get<{ items?: LookupItem[] } | LookupItem[]>('/api/v1/locations', { params })
      .subscribe(res => this.locations.set(Array.isArray(res) ? res : res.items || []));
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
          locationId: asset.locationId ?? null,
          serialNumber: asset.serialNumber ?? '',
          purchaseDate: asset.purchaseDate ? new Date(asset.purchaseDate) : null,
          notes: asset.notes ?? ''
        });

        this.form.get('assetTag')?.disable();
        this.loading.set(false);
      },
      error: () => {
        this.toast.showError('Error loading asset');
        this.loading.set(false);
        this.router.navigate(['/assets']);
      }
    });
  }

  /** Build a payload that matches CreateAssetCommand / UpdateAssetCommand JSON types. */
  private buildPayload(): CreateAssetCommand {
    const raw = this.form.getRawValue();
    const emptyToNull = (value: string | null | undefined): string | undefined =>
      value && String(value).trim() ? String(value).trim() : undefined;

    return {
      assetTag: String(raw.assetTag).trim(),
      name: String(raw.name).trim(),
      assetModelId: String(raw.assetModelId),
      statusLabelId: Number(raw.statusLabelId),
      purchaseCost: Number(raw.purchaseCost),
      locationId: emptyToNull(raw.locationId),
      serialNumber: emptyToNull(raw.serialNumber),
      purchaseDate: raw.purchaseDate
        ? new Date(raw.purchaseDate).toISOString()
        : undefined
    };
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.submitting.set(true);
    const payload = this.buildPayload();

    if (this.isEditMode && this.assetId) {
      const updateCommand: UpdateAssetCommand = {
        ...payload,
        id: this.assetId,
        notes: emptyToUndefined(this.form.getRawValue().notes)
      };

      this.assetService.updateAsset(this.assetId, updateCommand).subscribe({
        next: () => {
          this.toast.showSuccess('Asset updated successfully');
          this.router.navigate(['/assets', this.assetId]);
        },
        error: (err) => {
          this.toast.showError(`${err.error?.detail || err.message || 'Failed to update asset'}`);
          this.submitting.set(false);
        }
      });
    } else {
      // Body is the command itself — do NOT wrap as { command: ... }.
      // ASP.NET [FromBody] CreateAssetCommand command binds the whole JSON body.
      this.assetService.createAsset(payload).subscribe({
        next: (newId) => {
          this.toast.showSuccess('Asset created successfully');
          this.router.navigate(['/assets', newId]);
        },
        error: (err) => {
          const detail = err.error?.detail
            || (err.error?.errors && JSON.stringify(err.error.errors))
            || err.message
            || 'Failed to create asset';
          this.toast.showError(`${detail}`);
          this.submitting.set(false);
        }
      });
    }
  }
}

function emptyToUndefined(value: string | null | undefined): string | undefined {
  return value && String(value).trim() ? String(value).trim() : undefined;
}
