import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
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
    MatSnackBarModule
  ],
  templateUrl: './my-items.component.html',
  styleUrls: ['./my-items.component.scss']
})
export class MyItemsComponent implements OnInit {
  private userService = inject(UserService);
  private snackBar = inject(MatSnackBar);

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
        this.snackBar.open('Failed to load your items', 'Close', { duration: 4000 });
        this.loading.set(false);
      }
    });
  }

  forTab(key: keyof UserAssignedItemsDto): AssignedItemDto[] {
    return this.items()?.[key] ?? [];
  }
}
