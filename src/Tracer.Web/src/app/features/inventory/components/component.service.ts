import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface ComponentItem {
  id: number;
  name: string;
  companyId: string;
  totalQuantity: number;
  purchaseCost: number;
}

@Injectable({
  providedIn: 'root'
})
export class ComponentService {
  private http = inject(HttpClient);
  private apiUrl = '/api/v1/components';

  getComponents(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<ComponentItem>> {
    let params = new HttpParams()
      .set('pageNumber', page.toString())
      .set('pageSize', size.toString());

    if (sort) {
      params = params.set('sortColumn', sort).set('sortDirection', order);
    }

    return this.http.get<PaginatedResult<ComponentItem>>(this.apiUrl, { params });
  }

  createComponent(component: Partial<ComponentItem>): Observable<number> {
    return this.http.post<number>(this.apiUrl, component);
  }

  updateComponent(id: number, component: Partial<ComponentItem>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, component);
  }

  deleteComponent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
