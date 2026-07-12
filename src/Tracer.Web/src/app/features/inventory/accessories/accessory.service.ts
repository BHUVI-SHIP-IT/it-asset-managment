import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface Accessory {
  id: number;
  name: string;
  companyId: string;
  totalQuantity: number;
  purchaseCost: number;
}

@Injectable({
  providedIn: 'root'
})
export class AccessoryService {
  private http = inject(HttpClient);
  private apiUrl = '/api/v1/accessories';

  getAccessories(page: number, size: number, sort: string, order: string): Observable<PaginatedResult<Accessory>> {
    let params = new HttpParams()
      .set('pageNumber', page.toString())
      .set('pageSize', size.toString());

    if (sort) {
      params = params.set('sortColumn', sort).set('sortDirection', order);
    }

    return this.http.get<PaginatedResult<Accessory>>(this.apiUrl, { params });
  }

  createAccessory(accessory: Partial<Accessory>): Observable<number> {
    return this.http.post<number>(this.apiUrl, accessory);
  }

  updateAccessory(id: number, accessory: Partial<Accessory>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, accessory);
  }

  deleteAccessory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
