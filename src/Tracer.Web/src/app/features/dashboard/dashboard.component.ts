import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { DashboardService } from './dashboard.service';

interface StatCard {
  icon: string;
  label: string;
  value: string;
  trend: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatIconModule, MatButtonModule],
  template: `
    <div class="dashboard">
      <div class="dashboard-header">
        <h1 class="dashboard-title">Dashboard</h1>
        <p class="dashboard-subtitle">Overview of your IT asset portfolio</p>
      </div>

      <div class="stats-grid">
        @for (stat of stats; track stat.label) {
          <mat-card class="stat-card" [class]="'stat-' + stat.color">
            <mat-card-content>
              <div class="stat-icon-wrap">
                <mat-icon class="stat-icon">{{ stat.icon }}</mat-icon>
              </div>
              <div class="stat-info">
                <span class="stat-value">{{ stat.value }}</span>
                <span class="stat-label">{{ stat.label }}</span>
                <span class="stat-trend">{{ stat.trend }}</span>
              </div>
            </mat-card-content>
          </mat-card>
        }
      </div>

      <div class="quick-links">
        <h2>Quick Actions</h2>
        <div class="links-grid">
          <a mat-raised-button routerLink="/assets" color="primary">
            <mat-icon>devices</mat-icon> Manage Assets
          </a>
          <a mat-stroked-button routerLink="/master-data">
            <mat-icon>category</mat-icon> Master Data
          </a>
          <a mat-stroked-button routerLink="/settings">
            <mat-icon>settings</mat-icon> Settings
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
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 16px;
      margin-bottom: 32px;
    }
    .stat-card { border-radius: 12px !important; }
    .stat-card mat-card-content {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 20px !important;
    }
    .stat-icon-wrap {
      width: 52px; height: 52px;
      border-radius: 12px;
      display: flex; align-items: center; justify-content: center;
      background: rgba(103, 126, 234, 0.15);
    }
    .stat-blue .stat-icon-wrap { background: rgba(103,126,234,0.15); }
    .stat-green .stat-icon-wrap { background: rgba(76,175,80,0.15); }
    .stat-orange .stat-icon-wrap { background: rgba(255,152,0,0.15); }
    .stat-red .stat-icon-wrap { background: rgba(244,67,54,0.15); }
    .stat-icon { font-size: 28px; width: 28px; height: 28px; }
    .stat-blue .stat-icon { color: #667eea; }
    .stat-green .stat-icon { color: #4caf50; }
    .stat-orange .stat-icon { color: #ff9800; }
    .stat-red .stat-icon { color: #f44336; }
    .stat-info { display: flex; flex-direction: column; }
    .stat-value { font-size: 28px; font-weight: 700; line-height: 1; }
    .stat-label { font-size: 13px; color: var(--mat-sys-on-surface-variant); margin-top: 4px; }
    .stat-trend { font-size: 11px; color: #4caf50; margin-top: 2px; }
    h2 { font-size: 18px; font-weight: 600; margin-bottom: 16px; }
    .links-grid { display: flex; gap: 12px; flex-wrap: wrap; }
    .links-grid a { display: flex; align-items: center; gap: 8px; }
  `]
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  
  stats: StatCard[] = [
    { icon: 'devices', label: 'Total Assets', value: '—', trend: 'Loading...', color: 'blue' },
    { icon: 'check_circle', label: 'Active Assets', value: '—', trend: 'Loading...', color: 'green' },
    { icon: 'pending', label: 'Pending Checkout', value: '—', trend: 'Loading...', color: 'orange' },
    { icon: 'warning', label: 'Overdue Checkins', value: '—', trend: 'Loading...', color: 'red' },
  ];

  ngOnInit(): void {
    this.dashboardService.getMetrics().subscribe({
      next: (metrics) => {
        this.stats[0].value = metrics.totalAssets.toString();
        this.stats[0].trend = 'Up to date';
        
        this.stats[1].value = metrics.activeAssets.toString();
        this.stats[1].trend = 'Deployed / Deployable';
        
        this.stats[2].value = metrics.pendingCheckouts.toString();
        this.stats[2].trend = 'Awaiting deployment';
        
        this.stats[3].value = metrics.overdueCheckins.toString();
        this.stats[3].trend = 'Requires attention';
      },
      error: (err) => {
        console.error('Failed to load dashboard metrics', err);
        this.stats.forEach(s => s.trend = 'Error loading');
      }
    });
  }
}
