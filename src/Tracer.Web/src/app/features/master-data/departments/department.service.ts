import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../shared/components/base-table/base-table.component';

export interface Department {
  id: string;
  name: string;
}

export interface CreateDepartmentCommand {
  name: string;
}

export interface UpdateDepartmentCommand {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private http = inject(HttpClient);
  private baseUrl = '/api/v1/departments';

  getDepartments(pageNumber: number, pageSize: number, sortColumn?: string, sortDirection?: string): Observable<PaginatedResult<Department>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (sortColumn) {
      params = params.set('sortColumn', sortColumn);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }

    return this.http.get<PaginatedResult<Department>>(this.baseUrl, { params });
  }

  getDepartment(id: string): Observable<Department> {
    return this.http.get<Department>(`${this.baseUrl}/${id}`);
  }

  createDepartment(command: CreateDepartmentCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, command);
  }

  updateDepartment(id: string, command: UpdateDepartmentCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, command);
  }

  deleteDepartment(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
