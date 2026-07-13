import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ToastService } from '../../core/ui/toast.service';
import { AuthService } from '../../core/auth/auth.service';
import { Permissions } from '../../core/auth/permissions';
import { ConfirmDialogService } from '../../shared/components/confirm-dialog/confirm-dialog.service';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';
import { RequestService } from '../requests/request.service';
import {
  AssignedItemDto,
  UserAssignedItemsDto,
  UserService
} from '../users/user.service';

@Component({
  selector: 'app-my-items',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTabsModule,
    MatCardModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    HasPermissionDirective,
  ],
  templateUrl: './my-items.component.html',
  styleUrls: ['./my-items.component.scss']
})
export class MyItemsComponent implements OnInit {
  private userService = inject(UserService);
  private toast = inject(ToastService);
  private auth = inject(AuthService);
  private confirmDialog = inject(ConfirmDialogService);
  private requestService = inject(RequestService);

  readonly permissions = Permissions;
  readonly returnableTabs = new Set(['assets', 'consumables', 'components', 'accessories']);

  loading = signal(true);
  items = signal<UserAssignedItemsDto | null>(null);

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(): void {
    this.loading.set(true);
    this.userService.getMyAssignedItems().subscribe({
      next: data => {
        this.items.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.toast.showError('Failed to load your items');
        this.loading.set(false);
      }
    });
  }

  forTab(key: keyof UserAssignedItemsDto): AssignedItemDto[] {
    return this.items()?.[key] ?? [];
  }

  columnsFor(tabKey: string): string[] {
    return this.canReturnOnTab(tabKey)
      ? ['name', 'identifier', 'assignedAtUtc', 'status', 'actions']
      : ['name', 'identifier', 'assignedAtUtc', 'status'];
  }

  canReturnOnTab(tabKey: string): boolean {
    return this.returnableTabs.has(tabKey);
  }

  /** Only link to admin detail pages when the user can view that resource. */
  canOpenDetail(row: AssignedItemDto): boolean {
    if (!row.detailPath) return false;
    if (row.detailPath.startsWith('/assets/')) {
      return this.auth.hasPermission(Permissions.Assets.View);
    }
    return true;
  }

  requestReturn(row: AssignedItemDto): void {
    if (row.itemType === 'License') {
      return;
    }

    this.confirmDialog
      .open({
        title: 'Return item',
        message: `Submit a return request for '${row.name}'? An admin must approve before the item is unassigned.`,
        confirmText: 'Request Return'
      })
      .subscribe(confirmed => {
        if (!confirmed) {
          return;
        }
        this.requestService
          .create({
            type: 'Return',
            itemId: row.id,
            itemKind: row.itemType,
            quantity: null,
            notes: null
          })
          .subscribe({
            next: () => {
              this.toast.showSuccess('Return request submitted');
            },
            error: err => {
              const detail = err?.error?.detail || err?.message || 'Failed to submit return request';
              this.toast.showError(`${detail}`);
            }
          });
      });
  }
}
