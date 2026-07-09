import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface Supplier {
  id: string;
  name: string;
  contactName?: string;
  phone?: string;
  email?: string;
}

export interface CreateSupplierCommand {
  name: string;
  contactName?: string;
  phone?: string;
  email?: string;
}

export interface UpdateSupplierCommand {
  id: string;
  name: string;
  contactName?: string;
  phone?: string;
  email?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SupplierService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/suppliers';

  getSuppliers(pageNumber: number, pageSize: number, sortColumn?: string, sortDirection?: string): Observable<PaginatedResult<Supplier>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) {
      params = params.set('sortColumn', sortColumn);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PaginatedResult<Supplier>>(this.baseUrl, { params });
  }

  getSupplier(id: string): Observable<Supplier> {
    return this.http.get<Supplier>(`${this.baseUrl}/${id}`);
  }

  createSupplier(command: CreateSupplierCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  updateSupplier(id: string, command: UpdateSupplierCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteSupplier(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
