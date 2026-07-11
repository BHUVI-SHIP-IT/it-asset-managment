import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DepreciationDto {
  id: string;
  name: string;
  months: number;
  minimumValue: number;
  companyId: string;
}

export interface ReportExportDto {
  id: string;
  reportType: string;
  requestedAt: string;
  completedAt?: string;
  status: string;
  s3Url?: string;
}

@Injectable({
  providedIn: 'root'
})
export class FinancialsService {
  private http = inject(HttpClient);
  private apiUrl = '/api/v1';

  getDepreciationSchedules(): Observable<DepreciationDto[]> {
    return this.http.get<DepreciationDto[]>(`${this.apiUrl}/depreciation`);
  }

  getReports(): Observable<ReportExportDto[]> {
    return this.http.get<ReportExportDto[]>(`${this.apiUrl}/reports`);
  }

  requestReport(reportType: string): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/reports`, { reportType });
  }

  downloadReport(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/reports/${id}/download`, { responseType: 'blob' });
  }
}
