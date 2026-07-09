import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardMetricsDto {
  totalAssets: number;
  activeAssets: number;
  pendingCheckouts: number;
  overdueCheckins: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/dashboard';

  getMetrics(): Observable<DashboardMetricsDto> {
    return this.http.get<DashboardMetricsDto>(`${this.baseUrl}/metrics`);
  }
}
