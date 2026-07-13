import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardMetricsDto {
  totalAssets: number;
  activeAssets: number;
  pendingCheckouts: number;
  overdueCheckins: number;
  pendingApprovals: number;
}

export interface UserDashboardSummaryDto {
  assignedCounts: {
    assets: number;
    consumables: number;
    components: number;
    licenses: number;
    accessories: number;
  };
  requestCounts: {
    pending: number;
    approved: number;
    rejected: number;
  };
  attentionItems: Array<{
    kind: string;
    title: string;
    detail: string;
    dueAtUtc: string | null;
  }>;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);

  /** Org-wide metrics — requires Assets.View (admin). Do not call for end users. */
  getMetrics(): Observable<DashboardMetricsDto> {
    return this.http.get<DashboardMetricsDto>('/api/v1/dashboard/metrics');
  }

  /** Authenticated-user summary only — never hits admin/org endpoints. */
  getMySummary(): Observable<UserDashboardSummaryDto> {
    return this.http.get<UserDashboardSummaryDto>('/api/v1/me/summary');
  }
}
