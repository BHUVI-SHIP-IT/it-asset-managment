import { Permissions } from '../../../core/auth/permissions';
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { Observable } from 'rxjs';

import { BaseTableComponent, PaginatedResult } from '../../../shared/components/base-table/base-table.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { UserDto, UserService } from '../user.service';
import { UserFormDialogComponent } from '../user-form-dialog/user-form-dialog.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSnackBarModule,
    MatChipsModule,
    HasPermissionDirective
  ],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent extends BaseTableComponent<UserDto> implements OnInit {
  readonly permissions = Permissions;

  private userService = inject(UserService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  displayedColumns = ['fullName', 'email', 'roleName', 'isActive'];

  ngOnInit(): void {
    this.loadData();
  }

  protected override fetchData(page: number, size: number, _sort: string, _order: string): Observable<PaginatedResult<UserDto>> {
    return this.userService.getUsers(page, size);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(UserFormDialogComponent, { width: '480px', data: {} });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      this.userService.createUser(result).subscribe({
        next: () => {
          this.snackBar.open('User created successfully', 'Close', { duration: 3000 });
          this.loadData();
        },
        error: (err) => {
          const detail = err?.error?.detail || err?.message || 'Failed to create user';
          this.snackBar.open(`Error: ${detail}`, 'Close', { duration: 5000 });
        }
      });
    });
  }
}
