import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Permissions } from '../../../core/auth/permissions';
import { ToastService } from '../../../core/ui/toast.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import {
  RequestCatalogItemDto,
  RequestDto,
  RequestService
} from '../request.service';

@Component({
  selector: 'app-my-requests',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatTableModule,
    MatProgressSpinnerModule,
    HasPermissionDirective
  ],
  templateUrl: './my-requests.component.html',
  styleUrls: ['./my-requests.component.scss']
})
export class MyRequestsComponent implements OnInit {
  readonly permissions = Permissions;
  private requestService = inject(RequestService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  loading = signal(true);
  submitting = signal(false);
  requests = signal<RequestDto[]>([]);
  catalog = signal<RequestCatalogItemDto[]>([]);
  displayedColumns = ['type', 'itemName', 'quantity', 'status', 'requestedAtUtc', 'notes'];

  readonly types = [
    'Asset',
    'Consumable',
    'Component',
    'Accessory',
    'LicenseRenewal'
  ];

  form = this.fb.nonNullable.group({
    type: ['Asset', Validators.required],
    itemId: ['', Validators.required],
    quantity: [1 as number | null],
    notes: ['']
  });

  ngOnInit(): void {
    this.loadMine();
    this.loadCatalog(this.form.controls.type.value);
    this.form.controls.type.valueChanges.subscribe(type => {
      this.form.controls.itemId.setValue('');
      this.loadCatalog(type);
    });
  }

  loadMine(): void {
    this.loading.set(true);
    this.requestService.getMine().subscribe({
      next: rows => {
        this.requests.set(rows);
        this.loading.set(false);
      },
      error: () => {
        this.toast.showError('Failed to load requests');
        this.loading.set(false);
      }
    });
  }

  loadCatalog(type: string): void {
    this.requestService.catalog(type).subscribe({
      next: items => this.catalog.set(items),
      error: () => this.catalog.set([])
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.submitting.set(true);
    this.requestService.create({
      type: value.type,
      itemId: value.itemId,
      quantity: value.type === 'Consumable' ? value.quantity : null,
      notes: value.notes || null
    }).subscribe({
      next: () => {
        this.toast.showSuccess('Request submitted');
        this.submitting.set(false);
        this.form.patchValue({ itemId: '', notes: '', quantity: 1 });
        this.loadMine();
      },
      error: err => {
        const detail = err?.error?.detail || 'Failed to submit request';
        this.toast.showError(detail);
        this.submitting.set(false);
      }
    });
  }
}
