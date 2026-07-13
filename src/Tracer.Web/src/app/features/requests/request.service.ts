import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RequestDto {
  id: string;
  type: string;
  requestedByUserId: string;
  requestedByName: string;
  itemId: string | null;
  itemName: string | null;
  quantity: number | null;
  status: string;
  requestedAtUtc: string;
  resolvedByUserId: string | null;
  resolvedByName: string | null;
  resolvedAtUtc: string | null;
  notes: string | null;
  resolutionNotes: string | null;
}

export interface RequestCatalogItemDto {
  id: string;
  name: string;
  extra: string | null;
}

export interface CreateRequestCommand {
  type: string;
  itemId: string;
  quantity?: number | null;
  notes?: string | null;
  itemKind?: string | null;
}

@Injectable({ providedIn: 'root' })
export class RequestService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/requests';

  create(command: CreateRequestCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  getMine(): Observable<RequestDto[]> {
    return this.http.get<RequestDto[]>(`${this.baseUrl}/mine`);
  }

  getAll(status?: string): Observable<RequestDto[]> {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<RequestDto[]>(this.baseUrl, { params });
  }

  catalog(type: string): Observable<RequestCatalogItemDto[]> {
    return this.http.get<RequestCatalogItemDto[]>(`${this.baseUrl}/catalog`, {
      params: new HttpParams().set('type', type)
    });
  }

  approve(id: string, notes?: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/approve`, { notes: notes ?? null });
  }

  reject(id: string, notes?: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/reject`, { notes: notes ?? null });
  }
}
