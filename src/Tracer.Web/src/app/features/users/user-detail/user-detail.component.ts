import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { ToastService } from '../../../core/ui/toast.service';
import { AuthService } from '../../../core/auth/auth.service';
import { Permissions } from '../../../core/auth/permissions';
import {
  AssignedItemDto,
  UserAssignedItemsDto,
  UserDto,
  UserService
} from '../user.service';

@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTableModule
  ],
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private userService = inject(UserService);
  private toast = inject(ToastService);
  private auth = inject(AuthService);

  user = signal<UserDto | null>(null);
  assignedItems = signal<UserAssignedItemsDto | null>(null);
  loading = signal(true);
  itemsLoading = signal(false);
  itemsError = signal<string | null>(null);

  displayedColumns: string[] = ['name', 'identifier', 'assignedAtUtc', 'status'];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadUser(id);
    }
  }

  loadUser(id: string): void {
    this.loading.set(true);
    this.userService.getUser(id).subscribe({
      next: (data) => {
        this.user.set(data);
        this.loading.set(false);
        this.loadAssignedItems(id);
      },
      error: () => {
        this.toast.showError('Error loading user');
        this.loading.set(false);
      }
    });
  }

  loadAssignedItems(id: string): void {
    this.itemsLoading.set(true);
    this.itemsError.set(null);
    this.userService.getAssignedItems(id).subscribe({
      next: (data) => {
        this.assignedItems.set(data);
        this.itemsLoading.set(false);
      },
      error: () => {
        this.itemsError.set('Unable to load assigned items.');
        this.itemsLoading.set(false);
      }
    });
  }

  itemsForTab(key: string): AssignedItemDto[] {
    const items = this.assignedItems();
    if (!items) {
      return [];
    }

    switch (key) {
      case 'assets':
        return items.assets;
      case 'consumables':
        return items.consumables;
      case 'components':
        return items.components;
      case 'accessories':
        return items.accessories ?? [];
      case 'licenses':
        return items.licenses;
      default:
        return [];
    }
  }

  canOpenDetail(row: AssignedItemDto): boolean {
    if (!row.detailPath) return false;
    if (row.detailPath.startsWith('/assets/')) {
      return this.auth.hasPermission(Permissions.Assets.View);
    }
    return true;
  }
}
