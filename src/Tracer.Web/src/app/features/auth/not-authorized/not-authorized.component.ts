import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-not-authorized',
  standalone: true,
  imports: [RouterModule, MatButtonModule, MatIconModule, MatCardModule],
  template: `
    <div class="page">
      <mat-card class="card">
        <mat-icon class="icon" aria-hidden="true">lock</mat-icon>
        <h1>Not Authorized</h1>
        <p>You do not have permission to view this page.</p>
        <a mat-raised-button color="primary" routerLink="/dashboard">
          <mat-icon>dashboard</mat-icon>
          Back to Dashboard
        </a>
      </mat-card>
    </div>
  `,
  styles: [`
    .page {
      display: flex;
      justify-content: center;
      padding: 48px 16px;
    }
    .card {
      max-width: 420px;
      width: 100%;
      text-align: center;
      padding: 32px 24px;
      border-radius: 12px !important;
    }
    .icon {
      font-size: 56px;
      width: 56px;
      height: 56px;
      color: var(--mat-sys-on-surface-variant);
      margin-bottom: 8px;
    }
    h1 {
      margin: 0 0 8px;
      font-size: 24px;
      font-weight: 700;
    }
    p {
      margin: 0 0 24px;
      color: var(--mat-sys-on-surface-variant);
    }
    a {
      display: inline-flex;
      align-items: center;
      gap: 8px;
    }
  `]
})
export class NotAuthorizedComponent {}
