import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { DashboardService, UserDashboardSummaryDto } from './dashboard.service';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';
import { Permissions } from '../../core/auth/permissions';

interface StatCard {
  icon: string;
  label: string;
  value: string;
  trend: string;
  color: string;
}

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    HasPermissionDirective,
  ],
  template: `
    <div class="dashboard">
      <div class="dashboard-header">
        <h1 class="dashboard-title">My Dashboard</h1>
        <p class="dashboard-subtitle">Your assigned items and requests</p>
      </div>

      <h2>My Assigned Items</h2>
      <div class="stats-grid">
        @for (stat of assignedStats(); track stat.label) {
          <mat-card class="stat-card" [class]="'stat-' + stat.color">
            <mat-card-content>
              <div class="stat-icon-wrap">
                <mat-icon class="stat-icon">{{ stat.icon }}</mat-icon>
              </div>
              <div class="stat-info">
                <span class="stat-value">{{ stat.value }}</span>
                <span class="stat-label">{{ stat.label }}</span>
              </div>
            </mat-card-content>
          </mat-card>
        }
      </div>

      <h2>My Requests</h2>
      <div class="stats-grid">
        @for (stat of requestStats(); track stat.label) {
          <mat-card class="stat-card" [class]="'stat-' + stat.color">
            <mat-card-content>
              <div class="stat-icon-wrap">
                <mat-icon class="stat-icon">{{ stat.icon }}</mat-icon>
              </div>
              <div class="stat-info">
                <span class="stat-value">{{ stat.value }}</span>
                <span class="stat-label">{{ stat.label }}</span>
              </div>
            </mat-card-content>
          </mat-card>
        }
      </div>

      @if (attentionItems().length > 0) {
        <h2>Needs Attention</h2>
        <mat-card class="attention-card">
          <mat-card-content>
            <ul class="attention-list">
              @for (item of attentionItems(); track item.title + item.kind) {
                <li class="attention-item" [class.urgent]="isUrgent(item.kind)">
                  <mat-icon>{{ attentionIcon(item.kind) }}</mat-icon>
                  <div>
                    <div class="attention-title">{{ item.title }}</div>
                    <div class="attention-detail">{{ item.detail }}</div>
                  </div>
                </li>
              }
            </ul>
          </mat-card-content>
        </mat-card>
      }

      <div class="quick-links">
        <h2>Quick Actions</h2>
        <div class="links-grid">
          <a mat-raised-button routerLink="/requests/mine" color="primary" *hasPermission="permissions.Requests.Create">
            <mat-icon>add_circle</mat-icon> Submit New Request
          </a>
          <a mat-stroked-button routerLink="/my-items">
            <mat-icon>inventory</mat-icon> View My Items
          </a>
          <a mat-stroked-button routerLink="/requests/mine" *hasPermission="permissions.Requests.ViewOwn">
            <mat-icon>history</mat-icon> View My Request History
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard { padding: 8px 0; }
    .dashboard-header { margin-bottom: 32px; }
    .dashboard-title { font-size: 28px; font-weight: 700; margin: 0 0 4px; }
    .dashboard-subtitle { color: var(--mat-sys-on-surface-variant); margin: 0; }
    h2 { font-size: 18px; font-weight: 600; margin: 24px 0 16px; }
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 16px;
      margin-bottom: 8px;
    }
    .stat-card { border-radius: 12px !important; }
    .stat-card mat-card-content {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 20px !important;
    }
    .stat-icon-wrap {
      width: 48px; height: 48px;
      border-radius: 12px;
      display: flex; align-items: center; justify-content: center;
    }
    .stat-blue .stat-icon-wrap { background: rgba(103,126,234,0.15); }
    .stat-green .stat-icon-wrap { background: rgba(76,175,80,0.15); }
    .stat-orange .stat-icon-wrap { background: rgba(255,152,0,0.15); }
    .stat-red .stat-icon-wrap { background: rgba(244,67,54,0.15); }
    .stat-teal .stat-icon-wrap { background: rgba(0,150,136,0.15); }
    .stat-icon { font-size: 24px; width: 24px; height: 24px; }
    .stat-blue .stat-icon { color: #667eea; }
    .stat-green .stat-icon { color: #4caf50; }
    .stat-orange .stat-icon { color: #ff9800; }
    .stat-red .stat-icon { color: #f44336; }
    .stat-teal .stat-icon { color: #009688; }
    .stat-info { display: flex; flex-direction: column; }
    .stat-value { font-size: 26px; font-weight: 700; line-height: 1; }
    .stat-label { font-size: 13px; color: var(--mat-sys-on-surface-variant); margin-top: 4px; }
    .attention-card { border-radius: 12px !important; margin-bottom: 24px; }
    .attention-list { list-style: none; margin: 0; padding: 0; display: flex; flex-direction: column; gap: 12px; }
    .attention-item { display: flex; gap: 12px; align-items: flex-start; }
    .attention-item mat-icon { color: #ff9800; }
    .attention-item.urgent mat-icon { color: #f44336; }
    .attention-title { font-weight: 600; }
    .attention-detail { font-size: 13px; color: var(--mat-sys-on-surface-variant); }
    .links-grid { display: flex; gap: 12px; flex-wrap: wrap; }
    .links-grid a { display: flex; align-items: center; gap: 8px; }
  `]
})
export class UserDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  readonly permissions = Permissions;

  assignedStats = signal<StatCard[]>([
    { icon: 'devices', label: 'Assets', value: '—', trend: '', color: 'blue' },
    { icon: 'inventory_2', label: 'Consumables', value: '—', trend: '', color: 'teal' },
    { icon: 'memory', label: 'Components', value: '—', trend: '', color: 'orange' },
    { icon: 'vpn_key', label: 'Licenses', value: '—', trend: '', color: 'green' },
  ]);

  requestStats = signal<StatCard[]>([
    { icon: 'hourglass_empty', label: 'Pending', value: '—', trend: '', color: 'orange' },
    { icon: 'check_circle', label: 'Approved', value: '—', trend: '', color: 'green' },
    { icon: 'cancel', label: 'Rejected', value: '—', trend: '', color: 'red' },
  ]);

  attentionItems = signal<UserDashboardSummaryDto['attentionItems']>([]);

  ngOnInit(): void {
    // Only user-scoped endpoint — never calls /dashboard/metrics.
    this.dashboardService.getMySummary().subscribe({
      next: (summary) => this.applySummary(summary),
      error: (err) => {
        console.error('Failed to load user dashboard summary', err);
        this.assignedStats.update(cards => cards.map(s => ({ ...s, value: '!' })));
        this.requestStats.update(cards => cards.map(s => ({ ...s, value: '!' })));
      }
    });
  }

  attentionIcon(kind: string): string {
    switch (kind) {
      case 'LicenseExpired':
      case 'LicenseExpiring':
        return 'vpn_key_off';
      case 'ReturnOverdue':
        return 'warning';
      default:
        return 'event';
    }
  }

  isUrgent(kind: string): boolean {
    return kind === 'LicenseExpired' || kind === 'ReturnOverdue';
  }

  private applySummary(summary: UserDashboardSummaryDto): void {
    const a = summary.assignedCounts;
    this.assignedStats.set([
      { icon: 'devices', label: 'Assets', value: a.assets.toString(), trend: '', color: 'blue' },
      { icon: 'inventory_2', label: 'Consumables', value: a.consumables.toString(), trend: '', color: 'teal' },
      { icon: 'memory', label: 'Components', value: a.components.toString(), trend: '', color: 'orange' },
      { icon: 'vpn_key', label: 'Licenses', value: a.licenses.toString(), trend: '', color: 'green' },
    ]);

    const r = summary.requestCounts;
    this.requestStats.set([
      { icon: 'hourglass_empty', label: 'Pending', value: r.pending.toString(), trend: '', color: 'orange' },
      { icon: 'check_circle', label: 'Approved', value: r.approved.toString(), trend: '', color: 'green' },
      { icon: 'cancel', label: 'Rejected', value: r.rejected.toString(), trend: '', color: 'red' },
    ]);

    this.attentionItems.set(summary.attentionItems ?? []);
  }
}
