import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface Manufacturer {
  id: string;
  name: string;
}

export interface CreateManufacturerCommand {
  name: string;
}

export interface UpdateManufacturerCommand {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class ManufacturerService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/manufacturers';

  getManufacturers(pageNumber: number, pageSize: number, sortColumn?: string, sortDirection?: string): Observable<PaginatedResult<Manufacturer>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) {
      params = params.set('sortColumn', sortColumn);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PaginatedResult<Manufacturer>>(this.baseUrl, { params });
  }

  getManufacturer(id: string): Observable<Manufacturer> {
    return this.http.get<Manufacturer>(`${this.baseUrl}/${id}`);
  }

  createManufacturer(command: CreateManufacturerCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  updateManufacturer(id: string, command: UpdateManufacturerCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteManufacturer(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
