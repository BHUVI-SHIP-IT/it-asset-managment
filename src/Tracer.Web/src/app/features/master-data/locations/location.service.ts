import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface Location {
  id: string;
  name: string;
  address?: string;
  city?: string;
  country?: string;
}

export interface CreateLocationCommand {
  name: string;
  address?: string;
  city?: string;
  country?: string;
}

export interface UpdateLocationCommand {
  id: string;
  name: string;
  address?: string;
  city?: string;
  country?: string;
}

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/locations';

  getLocations(pageNumber: number, pageSize: number, sortColumn?: string, sortDirection?: string): Observable<PaginatedResult<Location>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) {
      params = params.set('sortColumn', sortColumn);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PaginatedResult<Location>>(this.baseUrl, { params });
  }

  getLocation(id: string): Observable<Location> {
    return this.http.get<Location>(`${this.baseUrl}/${id}`);
  }

  createLocation(command: CreateLocationCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  updateLocation(id: string, command: UpdateLocationCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteLocation(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
