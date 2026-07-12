import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface StatusLabel {
  id: number;
  name: string;
  isDeployable: boolean;
  isPending: boolean;
  isArchived: boolean;
}

export interface CreateStatusLabelCommand {
  name: string;
  isDeployable: boolean;
  isPending: boolean;
  isArchived: boolean;
}

export interface UpdateStatusLabelCommand {
  id: number;
  name: string;
  isDeployable: boolean;
  isPending: boolean;
  isArchived: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class StatusLabelService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/status-labels';

  getStatusLabels(pageNumber: number, pageSize: number, sortColumn?: string, sortDirection?: string): Observable<PaginatedResult<StatusLabel>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) {
      params = params.set('sortColumn', sortColumn);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PaginatedResult<StatusLabel>>(this.baseUrl, { params });
  }

  getStatusLabel(id: number): Observable<StatusLabel> {
    return this.http.get<StatusLabel>(`${this.baseUrl}/${id}`);
  }

  createStatusLabel(command: CreateStatusLabelCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, command);
  }

  updateStatusLabel(id: number, command: UpdateStatusLabelCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteStatusLabel(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
