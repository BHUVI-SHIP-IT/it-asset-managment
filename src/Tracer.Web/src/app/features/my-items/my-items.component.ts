import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ToastService } from '../../core/ui/toast.service';
import { AuthService } from '../../core/auth/auth.service';
import { Permissions } from '../../core/auth/permissions';
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
  ],
  templateUrl: './my-items.component.html',
  styleUrls: ['./my-items.component.scss']
})
export class MyItemsComponent implements OnInit {
  private userService = inject(UserService);
  private toast = inject(ToastService);
  private auth = inject(AuthService);

  loading = signal(true);
  items = signal<UserAssignedItemsDto | null>(null);
  displayedColumns = ['name', 'identifier', 'assignedAtUtc', 'status'];

  ngOnInit(): void {
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

  /** Only link to admin detail pages when the user can view that resource. */
  canOpenDetail(row: AssignedItemDto): boolean {
    if (!row.detailPath) return false;
    if (row.detailPath.startsWith('/assets/')) {
      return this.auth.hasPermission(Permissions.Assets.View);
    }
    return true;
  }
}
