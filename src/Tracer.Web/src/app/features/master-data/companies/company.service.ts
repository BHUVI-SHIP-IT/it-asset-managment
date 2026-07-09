import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface Company {
  id: string;
  name: string;
}

export interface CreateCompanyCommand {
  name: string;
}

export interface UpdateCompanyCommand {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/companies';

  getCompanies(pageNumber: number, pageSize: number, sortColumn?: string, sortDirection?: string): Observable<PaginatedResult<Company>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) {
      params = params.set('sortColumn', sortColumn);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PaginatedResult<Company>>(this.baseUrl, { params });
  }

  getCompany(id: string): Observable<Company> {
    return this.http.get<Company>(`${this.baseUrl}/${id}`);
  }

  createCompany(command: CreateCompanyCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  updateCompany(id: string, command: UpdateCompanyCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteCompany(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
